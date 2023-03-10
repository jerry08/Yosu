using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.Util;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gress;
using Gress.Completable;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Yosu.Extensions;
using Yosu.Services;
using Yosu.Utils;
using Yosu.ViewModels.Components;
using Yosu.Views.BottomSheets;
using Yosu.Youtube.Core.Downloading;
using Yosu.Youtube.Core.Tagging;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Videos.Streams;

namespace Yosu.ViewModels;

public class YoutubeViewModel
{
    private readonly SettingsService _settingsService;
    private readonly PreferenceService _preferenceService;

    private readonly ResizableSemaphore _downloadSemaphore = new();

    private readonly VideoDownloader _videoDownloader = new();
    private readonly MediaTagInjector _mediaTagInjector = new();

    private IBottomSheetController? BottomSheetController;

    public static List<YoutubeDownloadViewModel> Downloads { get; set; } = new();

    //Tyrrrz single video
    public IReadOnlyList<VideoDownloadOption>? AvailableDownloadOptions { get; set; }

    public VideoDownloadOption? SelectedDownloadOption { get; set; }

    //Tyrrrz multiple videos
    public Container SelectedContainer { get; set; } = Container.Mp4;

    public IReadOnlyList<Container> AvailableContainers { get; } = new[]
    {
        //Container.Mp4,
        //Container.WebM,
        Container.Mp3,
        new Container("ogg")
    };

    public IReadOnlyList<VideoQualityPreference> AvailableVideoQualityPreferences { get; } =
        Enum.GetValues<VideoQualityPreference>().Reverse().ToArray();

    public VideoQualityPreference SelectedVideoQualityPreference { get; set; } = VideoQualityPreference.Highest;

    public YoutubeViewModel(
        SettingsService settingsService,
        PreferenceService preferenceService)
    {
        _settingsService = settingsService;
        _preferenceService = preferenceService;

        _preferenceService.Load();
    }

    public void EnqueueDownload(YoutubeDownloadViewModel download)
    {
        _preferenceService.Load();
        _preferenceService.Downloads.Add(DownloadItem.FromViewModel(download));
        _preferenceService.Save();

        download.BeginDownload();

        _downloadSemaphore.MaxCount = _settingsService.YoutubeParallelLimit;

        Task.Run(async () =>
        {
            try
            {
                //App.StartForeground();
                //await Task.Delay(10000);
                //App.StopForeground();
                //return;

                using var access = await _downloadSemaphore.AcquireAsync(download.CancellationToken);

                download.Status = DownloadStatus.Started;

                var downloadOption =
                    download.DownloadOption ??
                    await _videoDownloader.GetBestDownloadOptionAsync(
                        download.Video!.Id,
                        download.DownloadPreference!,
                        download.CancellationToken
                    );

                var progress = new ProgressReporter();
                progress.OnReport += (s, e) => download.PercentageProgress = e;

                download.IsProgressIndeterminate = false;

                await _videoDownloader.DownloadVideoAsync(
                    download.TempFilePath!,
                    download.Video!,
                    downloadOption,
                    progress,
                    download.CancellationToken
                );

                download.IsProgressIndeterminate = true;

                if (_settingsService.ShouldInjectTags)
                {
                    try
                    {
                        await _mediaTagInjector.InjectTagsAsync(
                            download.TempFilePath!,
                            download.Video!,
                            download.CancellationToken
                        );
                    }
                    catch
                    {
                        // Media tagging is not critical
                    }
                }

#if ANDROID
                if (Platform.CurrentActivity is not null)
                {
                    await Platform.CurrentActivity.CopyFileAsync(
                        download.TempFilePath!,
                        download.FilePath!,
                        download.CancellationToken
                    );
                }
#endif

                download.Status = DownloadStatus.Completed;
            }
            catch (Exception ex)
            {
                download.PercentageProgress = Percentage.FromValue(100);

                download.Status = ex is OperationCanceledException
                    ? DownloadStatus.Canceled
                    : DownloadStatus.Failed;

                // Short error message for YouTube-related errors, full for others
                download.ErrorMessage = ex is YoutubeExplodeException
                    ? ex.Message
                    : ex.ToString();

                try
                {
                    // Delete file
                    if (!string.IsNullOrEmpty(download.FilePath))
                        File.Delete(download.FilePath);
                }
                catch
                {
                    // Ignore
                }
            }
            finally
            {
                try
                {
                    // Delete temporary downloaded file
                    File.Delete(download.TempFilePath!);
                }
                catch
                {
                    // Ignore
                }

                download.EndDownload();
                download.Dispose();

                Downloads.Remove(download);

                if (Downloads.Count == 0)
                {
                    NotificationHelper.ShowCompletedNotification();
                    App.StopForeground();
                }
            }
        });
    }

    public async void ShowOptions(List<YoutubeDownloadViewModel> downloads)
    {
        await Task.Run(async () =>
        {
            try
            {
                //IView page = SelectedEntities.Count == 1
                //    ? new DownloadSingleYtOptionsView(this)
                //    : new DownloadMultipleYtOptionsView(this);

                if (downloads.Count == 1)
                {
                    UserDialogs.Instance.ShowLoading("Loading...");

                    var video = downloads.Single().Video!;

                    AvailableDownloadOptions = await _videoDownloader.GetDownloadOptionsAsync(video.Id);

                    // Nobody really use the other formats
                    AvailableDownloadOptions = AvailableDownloadOptions
                        .Where(x => x.Container == Container.Mp4
                            || x.Container == Container.Mp3).ToList();

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        var page = new DownloadSingleYtOptionsView(this, downloads);
                        BottomSheetController = Shell.Current.ShowBottomSheet(page, false);

                        UserDialogs.Instance.HideLoading();
                    });
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        var page = new DownloadMultipleYtOptionsView(this, downloads);
                        BottomSheetController = Shell.Current.ShowBottomSheet(page, false);
                    });
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.HideLoading();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Snackbar.Make(e.Message).Show());
            }
        });
    }

    public async void Download(List<YoutubeDownloadViewModel> downloads, object? opt)
    {
        if (opt is null)
            return;

        Downloads.AddRange(downloads);

        BottomSheetController?.Dismiss();
        BottomSheetController = null;

        App.StartForeground();

        for (int i = 0; i < Downloads.Count; i++)
        {
            Container selectedContainer = default!;

            switch (opt)
            {
                case VideoDownloadOption option:
                    selectedContainer = option.Container;
                    Downloads[i].DownloadOption = option;
                    break;
                //case VideoDownloadPreference option:
                //    selectedContainer = option.PreferredContainer;
                //    Downloads[i].DownloadPreference = option;
                //    break;
                case Container container:
                    selectedContainer = container;
                    Downloads[i].DownloadPreference = new VideoDownloadPreference(container, SelectedVideoQualityPreference);
                    break;
                case VideoQualityPreference preference:
                    selectedContainer = SelectedContainer;
                    Downloads[i].DownloadPreference = new VideoDownloadPreference(SelectedContainer, preference);
                    break;
            }

#if ANDROID
            var dirPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath;
#elif IOS || MACCATALYST
            var dirPath = "";
#endif

            var baseFilePath = Path.Combine(
                dirPath,
                FileNameTemplate.Apply(
                    _settingsService.FileNameTemplate,
                    Downloads[i].Video!,
                    selectedContainer,
                    (i + 1).ToString().PadLeft(Downloads.Count.ToString().Length, '0')
                )
            );

            if (_settingsService.ShouldSkipExistingFiles && File.Exists(baseFilePath))
                continue;

            var filePath = PathEx.EnsureUniquePath(baseFilePath);
            var extension = Path.GetExtension(baseFilePath);
            var tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{DateTime.Now.Ticks}{extension}");

            Downloads[i].FilePath = filePath;
            Downloads[i].TempFilePath = tempFilePath;

            EnqueueDownload(Downloads[i]);
        }

        await Toast.Make("Download started").Show();
    }

    public void RestartDownload(YoutubeDownloadViewModel download)
    {
        if (!App.IsOnline(false))
            return;

        Download(
            new() { download },
            (object?)download.DownloadOption ?? download.DownloadPreference
        );
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Berry.Maui.Controls;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using Gress;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Yosu.Data;
using Yosu.Services;
using Yosu.Utils;
using Yosu.Utils.Extensions;
using Yosu.ViewModels.Components;
using Yosu.Views;
using Yosu.Views.BottomSheets;
using Yosu.Youtube.Core.Downloading;
using Yosu.Youtube.Core.Tagging;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Videos.Streams;

namespace Yosu.ViewModels;

public class YoutubeViewModel
{
    private readonly SettingsService _settingsService;
    private readonly HistoryDatabase _historyDatabase;

    private readonly ResizableSemaphore _downloadSemaphore = new();

    private readonly VideoDownloader _videoDownloader = new();
    private readonly MediaTagInjector _mediaTagInjector = new();

    private BottomSheet? BottomSheet;

    public static List<YoutubeDownloadViewModel> Downloads { get; set; } = [];

    //Tyrrrz single video
    public IReadOnlyList<VideoDownloadOption>? AvailableDownloadOptions { get; set; }

    public VideoDownloadOption? SelectedDownloadOption { get; set; }

    //Tyrrrz multiple videos
    public Container SelectedContainer { get; set; } = Container.Mp4;

    public IReadOnlyList<Container> AvailableContainers { get; } =
        [Container.Mp4, Container.WebM, Container.Mp3, new Container("ogg")];

    public IReadOnlyList<VideoQualityPreference> AvailableVideoQualityPreferences { get; } =
        Enum.GetValues<VideoQualityPreference>().Reverse().ToArray();

    public VideoQualityPreference SelectedVideoQualityPreference { get; set; } =
        VideoQualityPreference.Highest;

    public YoutubeViewModel(SettingsService settingsService, HistoryDatabase historyDatabase)
    {
        _settingsService = settingsService;
        _historyDatabase = historyDatabase;

        _settingsService.Load();
    }

    public async void EnqueueDownload(YoutubeDownloadViewModel download)
    {
        await _historyDatabase.AddItemAsync(DownloadItem.From(download));

        Downloads.Add(download);

        download.BeginDownload();

        _downloadSemaphore.MaxCount = _settingsService.ParallelLimit;

        await Task.Run(async () =>
        {
            try
            {
                //App.StartForeground();
                //await Task.Delay(10000);
                //App.StopForeground();
                //return;

                using var access = await _downloadSemaphore.AcquireAsync(
                    download.CancellationToken
                );

                download.Status = DownloadStatus.Started;

                var downloadOption =
                    download.DownloadOption
                    ?? await _videoDownloader.GetBestDownloadOptionAsync(
                        download.Video!.Id,
                        download.DownloadPreference!,
                        download.CancellationToken
                    );

                var progress = new ProgressReporter();
                progress.OnReport += (_, e) => download.PercentageProgress = e;

                download.IsProgressIndeterminate = false;

                await _videoDownloader.DownloadVideoAsync(
                    download.TempFilePath!,
                    download.Video!,
                    downloadOption,
                    _settingsService.ShouldInjectSubtitles,
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

                download.Status =
                    ex is OperationCanceledException
                        ? DownloadStatus.Canceled
                        : DownloadStatus.Failed;

                // Short error message for YouTube-related errors, full for others
                download.ErrorMessage = ex is YoutubeExplodeException ? ex.Message : ex.ToString();

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

#if ANDROID
                if (Downloads.Count == 0)
                {
                    NotificationHelper.ShowCompletedNotification(
                        $"Saved to {Path.GetDirectoryName(download.FilePath)}"
                    );

                    //var test = ApplicationEx.IsRunning();
                    //var test2 = ApplicationEx.IsInBackground();
                    //var test3 = ApplicationEx.IsInForeground();
                    App.StopForeground();
                    //var test4 = ApplicationEx.IsRunning();
                }
#endif
            }
        });
    }

    public async void ShowOptions(List<YoutubeDownloadViewModel> downloads)
    {
        await Task.Run(async () =>
        {
            Popup? popup = null;

            try
            {
                //IView page = SelectedEntities.Count == 1
                //    ? new DownloadSingleYtOptionsView(this)
                //    : new DownloadMultipleYtOptionsView(this);

                if (downloads.Count == 1)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        popup = new LoadingPopup(
                            new LoadingPopupViewModel { LoadingText = "Loading..." }
                        );
                        Application.Current?.MainPage?.ShowPopup(popup);
                    });

                    var video = downloads.Single().Video!;

                    AvailableDownloadOptions = await _videoDownloader.GetDownloadOptionsAsync(
                        video.Id
                    );

                    // Nobody really use the other formats
                    AvailableDownloadOptions = AvailableDownloadOptions
                        .Where(x => x.Container == Container.Mp4 || x.Container == Container.Mp3)
                        .ToList();

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        BottomSheet = new DownloadSingleYtOptionsView(this, downloads);
                        popup?.Close();
                        await BottomSheet.ShowAsync();
                    });
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        BottomSheet = new DownloadMultipleYtOptionsView(this, downloads);
                        await BottomSheet.ShowAsync();
                    });
                }
            }
            catch (Exception e)
            {
                popup?.Close();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await App.AlertSvc.ShowAlertAsync("Error", e.Message);
                });
            }
        });
    }

    public async void Download(List<YoutubeDownloadViewModel> downloads, object? opt)
    {
        if (opt is null)
            return;

        //Downloads.AddRange(downloads);

        if (BottomSheet is not null)
        {
            await BottomSheet.DismissAsync();
            BottomSheet = null;
        }

        var started = false;

        for (int i = 0; i < downloads.Count; i++)
        {
            Container selectedContainer = default!;

            switch (opt)
            {
                case VideoDownloadOption option:
                    selectedContainer = option.Container;
                    downloads[i].DownloadOption = option;
                    break;
                //case VideoDownloadPreference option:
                //    selectedContainer = option.PreferredContainer;
                //    downloads[i].DownloadPreference = option;
                //    break;
                case Container container:
                    selectedContainer = container;
                    downloads[i].DownloadPreference = new VideoDownloadPreference(
                        container,
                        SelectedVideoQualityPreference
                    );
                    break;
                case VideoQualityPreference preference:
                    selectedContainer = SelectedContainer;
                    downloads[i].DownloadPreference = new VideoDownloadPreference(
                        SelectedContainer,
                        preference
                    );
                    break;
            }

#if ANDROID
            //var dirPath = Path.Combine(
            //    Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath,
            //    "Yosu"
            //);

            var dirPath = !string.IsNullOrWhiteSpace(_settingsService.DownloadDir)
                ? _settingsService.DownloadDir
                : Path.Combine(
                    Android
                        .OS.Environment.GetExternalStoragePublicDirectory(
                            Android.OS.Environment.DirectoryDownloads
                        )!
                        .AbsolutePath,
                    "Yosu"
                );
#elif IOS || MACCATALYST
            var dirPath = "";
#endif

            var baseFilePath = Path.Combine(
                dirPath,
                FileNameTemplate.Apply(
                    _settingsService.YoutubeFileNameTemplate,
                    downloads[i].Video!,
                    selectedContainer,
                    (i + 1).ToString().PadLeft(downloads.Count.ToString().Length, '0')
                )
            );

            if (_settingsService.ShouldSkipExistingFiles && FileEx.Exists(baseFilePath))
                continue;

            var filePath = PathEx.EnsureUniquePath(baseFilePath);
            var extension = Path.GetExtension(baseFilePath);
            var tempFilePath = Path.Combine(
                FileSystem.CacheDirectory,
                $"{DateTime.Now.Ticks}{extension}"
            );

            downloads[i].FilePath = filePath;
            downloads[i].TempFilePath = tempFilePath;

            if (!started)
            {
#if ANDROID
                App.StartForeground();
#endif
                await Toast.Make("Download started").Show();
                started = true;
            }

            EnqueueDownload(downloads[i]);
        }
    }

    public void RestartDownload(YoutubeDownloadViewModel download)
    {
        if (!App.IsOnline(false))
            return;

        Download([download], (object?)download.DownloadOption ?? download.DownloadPreference);
    }
}

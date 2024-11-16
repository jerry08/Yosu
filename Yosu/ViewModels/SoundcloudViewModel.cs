using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gress;
using Microsoft.Maui.ApplicationModel;
using SoundCloudExplode.Exceptions;
using Yosu.Data;
using Yosu.Services;
using Yosu.Soundcloud.Core.Downloading;
using Yosu.Soundcloud.Core.Tagging;
using Yosu.Utils;
using Yosu.Utils.Extensions;
using Yosu.ViewModels.Components;

namespace Yosu.ViewModels;

public class SoundcloudViewModel
{
    private readonly SettingsService _settingsService;
    private readonly HistoryDatabase _historyDatabase;

    private readonly ResizableSemaphore _downloadSemaphore = new();

    private readonly TrackDownloader _trackDownloader = new();
    private readonly MediaTagInjector _mediaTagInjector = new();

    public static List<SoundcloudDownloadViewModel> Downloads { get; set; } = [];

    public SoundcloudViewModel(SettingsService settingsService, HistoryDatabase historyDatabase)
    {
        _settingsService = settingsService;
        _settingsService.Load();

        _historyDatabase = historyDatabase;
    }

    public void EnqueueDownloads(IEnumerable<SoundcloudDownloadViewModel> downloads)
    {
#if ANDROID
        App.StartForeground();
#endif

        foreach (var download in downloads)
        {
            var fileName = FileNameTemplate.Apply(
                _settingsService.SoundCloudFileNameTemplate,
                download.Track!,
                "mp3"
            );

#if ANDROID
            if (!string.IsNullOrWhiteSpace(_settingsService.DownloadDir))
            {
                download.FilePath = Path.Combine(_settingsService.DownloadDir, fileName);
            }
            else
            {
                download.FilePath = Path.Combine(
                    Android
                        .OS.Environment.GetExternalStoragePublicDirectory(
                            Android.OS.Environment.DirectoryDownloads
                        )!
                        .AbsolutePath,
                    //Android.OS.Environment.ExternalStorageDirectory!.AbsolutePath,
                    "Yosu",
                    fileName
                );
            }
#endif

            if (_settingsService.ShouldSkipExistingFiles && FileEx.Exists(download.FilePath))
                continue;

            EnqueueDownload(download);
        }
    }

    private async void EnqueueDownload(SoundcloudDownloadViewModel download)
    {
        await _historyDatabase.AddItemAsync(DownloadItem.From(download));

        Downloads.Add(download);

        download.BeginDownload();

        _downloadSemaphore.MaxCount = _settingsService.ParallelLimit;

        await Task.Run(async () =>
        {
            try
            {
                using var access = await _downloadSemaphore.AcquireAsync(
                    download.CancellationToken
                );

                download.Status = DownloadStatus.Started;
                download.IsProgressIndeterminate = false;

                var progress = new ProgressReporter();
                progress.OnReport += (_, e) => download.PercentageProgress = e;

                await _trackDownloader.DownloadAsync(
                    download.TempFilePath!,
                    download.Track!,
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
                            download.Track!,
                            download.CancellationToken
                        );
                    }
                    catch
                    {
                        // Media tagging is not critical
                    }
                }

                download.Status = DownloadStatus.Completed;

#if ANDROID
                if (Platform.CurrentActivity is not null)
                {
                    await Platform.CurrentActivity.CopyFileAsync(
                        download.TempFilePath,
                        download.FilePath!,
                        download.CancellationToken
                    );
                }
#endif
            }
            catch (Exception ex)
            {
                download.PercentageProgress = Percentage.FromValue(100);

                download.Status =
                    ex is OperationCanceledException
                        ? DownloadStatus.Canceled
                        : DownloadStatus.Failed;

                // Short error message for SoundCloud-related errors, full for others
                download.ErrorMessage =
                    ex is SoundcloudExplodeException ? ex.Message : ex.ToString();

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

                    App.StopForeground();
                }
#endif
            }
        });
    }

    public void RestartDownload(SoundcloudDownloadViewModel download)
    {
        if (!App.IsOnline(false))
            return;

        EnqueueDownload(download);
    }
}

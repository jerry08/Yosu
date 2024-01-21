using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gress;
using Microsoft.Maui.ApplicationModel;
using SoundCloudExplode.Exceptions;
using Yosu.Extensions;
using Yosu.Services;
using Yosu.Soundcloud.Core.Downloading;
using Yosu.Soundcloud.Core.Tagging;
using Yosu.Utils;
using Yosu.ViewModels.Components;

namespace Yosu.ViewModels;

public class SoundcloudViewModel
{
    private readonly SettingsService _settingsService;
    private readonly PreferenceService _preferenceService;

    private readonly ResizableSemaphore _downloadSemaphore = new();

    private readonly TrackDownloader _trackDownloader = new();
    private readonly MediaTagInjector _mediaTagInjector = new();

    public static List<SoundcloudDownloadViewModel> Downloads { get; set; } = new();

    public SoundcloudViewModel(SettingsService settingsService, PreferenceService preference)
    {
        _settingsService = settingsService;
        _preferenceService = preference;

        _settingsService.Load();
        _preferenceService.Load();
    }

    public void EnqueueDownloads(IEnumerable<SoundcloudDownloadViewModel> downloads)
    {
        App.StartForeground();

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

    private void EnqueueDownload(SoundcloudDownloadViewModel download)
    {
        _preferenceService.Load();
        _preferenceService.Downloads.Add(DownloadItem.FromViewModel(download));
        _preferenceService.Save();

        Downloads.Add(download);

        download.BeginDownload();

        _downloadSemaphore.MaxCount = _settingsService.ParallelLimit;

        Task.Run(async () =>
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

                // Update download information
                //await _preference.LoadAsync();

                //var existingDownload = _preference.Downloads
                //    .FirstOrDefault(x => x.SourceType == SourceType.Soundcloud &&
                //        x.Key == download.Track?.PermalinkUrl?.ToString());
                //
                //if (existingDownload is not null)
                //{
                //    existingDownload = DownloadItem.FromViewModel(download);
                //    await _preference.SaveAsync();
                //}
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

                if (Downloads.Count == 0)
                {
                    NotificationHelper.ShowCompletedNotification(
                        $"Saved to {Path.GetDirectoryName(download.FilePath)}"
                    );

                    App.StopForeground();
                }
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

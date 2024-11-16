﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gress;
using Microsoft.Maui.ApplicationModel;
using SpotifyExplode.Exceptions;
using Yosu.Data;
using Yosu.Services;
using Yosu.Spotify.Core.Downloading;
using Yosu.Spotify.Core.Tagging;
using Yosu.Utils;
using Yosu.Utils.Extensions;
using Yosu.ViewModels.Components;

namespace Yosu.ViewModels;

public class SpotifyViewModel
{
    private readonly SettingsService _settingsService;
    private readonly HistoryDatabase _historyDatabase;

    private readonly ResizableSemaphore _downloadSemaphore = new();

    private readonly TrackDownloader _trackDownloader = new();
    private readonly MediaTagInjector _mediaTagInjector = new();

    public static List<SpotifyDownloadViewModel> Downloads { get; set; } = [];

    public SpotifyViewModel(SettingsService settingsService, HistoryDatabase historyDatabase)
    {
        _settingsService = settingsService;
        _historyDatabase = historyDatabase;

        _settingsService.Load();
    }

    public void EnqueueDownloads(IEnumerable<SpotifyDownloadViewModel> downloads)
    {
#if ANDROID
        App.StartForeground();
#endif

        foreach (var download in downloads)
        {
            var fileName = FileNameTemplate.Apply(
                _settingsService.SpotifyFileNameTemplate,
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

    private async void EnqueueDownload(SpotifyDownloadViewModel download)
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

                var progress = new ProgressReporter();
                progress.OnReport += (s, e) =>
                {
                    if (download.PercentageProgress.Fraction > 0)
                        download.IsProgressIndeterminate = false;

                    download.PercentageProgress = e;
                };

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

                // Short error message for Spotify-related errors, full for others
                download.ErrorMessage = ex is SpotifyExplodeException ? ex.Message : ex.ToString();

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

    public void RestartDownload(SpotifyDownloadViewModel download)
    {
        if (!App.IsOnline(false))
            return;

        EnqueueDownload(download);
    }
}

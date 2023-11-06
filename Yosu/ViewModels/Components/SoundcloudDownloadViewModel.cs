using System;
using System.IO;
using Microsoft.Maui.Storage;
using SoundCloudExplode.Tracks;

namespace Yosu.ViewModels.Components;

public class SoundcloudDownloadViewModel : DownloadViewModelBase
{
    public long? Key => Track?.Id;

    public Track? Track { get; set; }

    public SoundcloudDownloadViewModel()
    {
        TempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{DateTime.Now.Ticks}.mp3");

        //PropertyChanged += (s, e) =>
        //{
        //    IsProgressIndeterminate = Progress.Current.Fraction is <= 0 or >= 1;
        //    IsCanceledOrFailed = Status is DownloadStatus.Canceled or DownloadStatus.Failed;
        //    CanCancel = Status is DownloadStatus.Enqueued or DownloadStatus.Started;
        //};
    }
}

using System;
using AngleSharp.Io;

namespace Yosu.ViewModels.Components;

public class DownloadItem
{
    public string Key { get; set; } = default!;

    public DateTime DownloadDate { get; set; }

    public object? Entity { get; set; }

    public DownloadStatus DownloadStatus { get; set; }

    public SourceType SourceType { get; set; }

    public string? ErrorMessage { get; set; }

    public DownloadItem()
    {
        DownloadDate = DateTime.Now;
        DownloadStatus = DownloadStatus.Enqueued;
    }

    public DownloadItem(object entity) : this()
    {
        Entity = entity;
    }

    public static DownloadItem FromViewModel(DownloadViewModelBase viewModel)
    {
        var downloadItem = new DownloadItem();

        switch (viewModel)
        {
            case YoutubeDownloadViewModel download:
                downloadItem.Key = download.Video?.Url ?? "";
                downloadItem.Entity = download.Video;
                downloadItem.SourceType = SourceType.Youtube;
                break;

            case SoundcloudDownloadViewModel download:
                downloadItem.Key = download.Track?.PermalinkUrl?.ToString() ?? "";
                downloadItem.Entity = download.Track;
                downloadItem.SourceType = SourceType.Soundcloud;
                break;

            case SpotifyDownloadViewModel download:
                downloadItem.Key = download.Track?.Url ?? "";
                downloadItem.Entity = download.Track;
                downloadItem.SourceType = SourceType.Spotify;
                break;
        }

        return downloadItem;
    }
}
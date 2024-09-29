﻿using System;
using System.Linq;
using System.Text.Json.Serialization;
using SQLite;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using SoundCloudTrack = SoundCloudExplode.Tracks.Track;
using SpotifyTrack = SpotifyExplode.Tracks.Track;

namespace Yosu.ViewModels.Components;

public class DownloadItem
{
    public int Id { get; set; }

    public string Key { get; set; } = default!;

    public DateTime DownloadDate { get; set; }

    public string? Url { get; set; }

    public string? Title { get; set; }

    public string? Author { get; set; }

    public string? ImageUrl { get; set; }

    public TimeSpan? Duration { get; set; }

    public long? PlaybackCount { get; set; }

    // Applies to SoundCloud only
    public DateTimeOffset? DisplayDate { get; set; }

    [Ignore]
    [JsonIgnore]
    public object? Entity { get; set; }

    public DownloadStatus DownloadStatus { get; set; }

    public SourceType SourceType { get; set; }

    public string? ErrorMessage { get; set; }

    public DownloadItem()
    {
        DownloadDate = DateTime.Now;
        DownloadStatus = DownloadStatus.Enqueued;
    }

    public static DownloadItem From(SoundcloudDownloadViewModel viewModel) =>
        new()
        {
            Key = viewModel.Track?.PermalinkUrl?.ToString() ?? string.Empty,
            Entity = viewModel.Track,
            SourceType = SourceType.Soundcloud,
            ImageUrl = viewModel.Track?.ArtworkUrl?.ToString() ?? string.Empty,
            Title = viewModel.Track?.Title,
            Author = viewModel.Track?.User?.Username,
            PlaybackCount = viewModel.Track?.PlaybackCount,
            Duration = TimeSpan.FromMilliseconds(viewModel.Track?.Duration ?? 0),
            DisplayDate = viewModel.Track?.DisplayDate,
        };

    public static DownloadItem From(SpotifyDownloadViewModel viewModel) =>
        new()
        {
            Key = viewModel.Track?.Url?.ToString() ?? string.Empty,
            Entity = viewModel.Track,
            SourceType = SourceType.Spotify,
            Title = viewModel.Track?.Title,
            Author = viewModel.Track?.Artists.FirstOrDefault()?.Name,
            Duration = TimeSpan.FromMilliseconds(viewModel.Track?.DurationMs ?? 0),
            ImageUrl = viewModel.Track?.Album.Images.FirstOrDefault()?.Url,
        };

    public static DownloadItem From(YoutubeDownloadViewModel viewModel) =>
        new()
        {
            Key = viewModel.Video?.Url ?? string.Empty,
            Entity = viewModel.Video,
            SourceType = SourceType.Youtube,
            Title = viewModel.Video?.Title,
            Duration = viewModel.Video?.Duration,
            Author = viewModel.Video?.Author.ChannelTitle,
            ImageUrl = viewModel.Video?.Thumbnails.TryGetWithHighestResolution()?.Url,
        };
}

public static class DownloadItemExtensions
{
    public static void SetEntity(this DownloadItem download)
    {
        switch (download.SourceType)
        {
            case SourceType.Youtube:
                download.Entity = new YoutubeDownloadViewModel()
                {
                    Video = new Video(
                        download.Key,
                        download.Title ?? string.Empty,
                        new(default!, download.Author ?? string.Empty),
                        default!,
                        default!,
                        download.Duration,
                        [new(download.ImageUrl ?? string.Empty, new())],
                        default!,
                        default!
                    ),
                };
                break;

            case SourceType.Soundcloud:
                download.Entity = new SoundcloudDownloadViewModel()
                {
                    Track = new SoundCloudTrack()
                    {
                        Title = download.Title,
                        ArtworkUrl = !string.IsNullOrEmpty(download.ImageUrl)
                            ? new Uri(download.ImageUrl)
                            : null,
                        User = new() { Username = download.Author },
                        PlaybackCount = download.PlaybackCount,
                        Duration = (long?)download.Duration?.TotalMilliseconds,
                        DisplayDate = download.DisplayDate ?? default,
                    },
                };
                break;

            case SourceType.Spotify:
                download.Entity = new SpotifyDownloadViewModel()
                {
                    Track = new SpotifyTrack()
                    {
                        Title = download.Title ?? string.Empty,
                        //PreviewUrl = download.ImageUrl ?? string.Empty,
                        Album = new()
                        {
                            Images = [new() { Url = download.ImageUrl ?? string.Empty }],
                        },
                        Artists = [new() { Name = download.Author! }],
                        DurationMs = (long)download.Duration!.Value.TotalMilliseconds,
                    },
                };
                break;
        }
    }
}

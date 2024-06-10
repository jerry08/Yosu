using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using Httpz;
using SpotifyExplode;
using SpotifyExplode.Tracks;
using TagLib.Riff;
using Y2mateApi;
using Yosu.Spotify.Core.Utils;
using Yosu.Youtube.Core.Downloading;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace Yosu.Spotify.Core.Downloading;

public class TrackDownloader
{
    private readonly SpotifyClient _spotify = new();
    private readonly Y2mateClient y2mate = new();

    /*private readonly YoutubeClient _youtube = new();
    public async Task FindBestMatchVideoIdAsync(
        Track track,
        CancellationToken cancellationToken = default)
    {
        var results = await _youtube.Search.GetVideosAsync(
            $"{track.Title} - {track.Artists.FirstOrDefault()?.Name}",
            cancellationToken
        ).CollectAsync(40);

        var video = results.OrderBy(x => LevenshteinDistance.Compute(
            x.Title.ToLower(), track.Title.ToLower())
        ).ThenBy().FirstOrDefault();

        var video2 = results
            .Aggregate((x, y) => Math.Abs(x.Duration!.Value.TotalMilliseconds - track.DurationMs) < Math.Abs(y.Duration!.Value.TotalMilliseconds - track.DurationMs) ? x : y);
    }*/

    /*private readonly YoutubeClient _youtube = new();
    private readonly VideoDownloader _videoDownloader = new();

    public async Task DownloadAsync(
        string filePath,
        Track track,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var results = await _youtube.Search.GetVideosAsync(
            $"{track.Title} - {track.Artists.FirstOrDefault()?.Name}",
            cancellationToken
        ).CollectAsync(40);

        var video = results.OrderBy(x => LevenshteinDistance.Compute(
            x.Title.ToLower(), track.Title.ToLower())
        ).FirstOrDefault();

        // Find best match video by duration
        //var video = results
        //    .Aggregate((x, y) => Math.Abs(x.Duration!.Value.TotalMilliseconds - track.DurationMs) < Math.Abs(y.Duration!.Value.TotalMilliseconds - track.DurationMs) ? x : y);

        var downloadPreference = new VideoDownloadPreference(Container.Mp3, VideoQualityPreference.Highest);

        var downloadOption = await _videoDownloader.GetBestDownloadOptionAsync(
            video!.Id,
            downloadPreference,
            cancellationToken
        );

        await _videoDownloader.DownloadVideoAsync(
            filePath,
            video,
            downloadOption,
            progress,
            cancellationToken
        );

        //var Diversion = 60 * 1000;
        //
        //var downloadUrl = "";
        //
        //await foreach (var result in _youtube.Search.GetVideosAsync($"{track.Title} - {track.Artists.FirstOrDefault()?.Name}"))
        //{
        //    int resultAudioDuration = (int)result.Duration.Value.TotalMilliseconds;
        //    if (track.DurationMs - Diversion > resultAudioDuration || resultAudioDuration > track.DurationMs + Diversion)
        //    {
        //        continue;
        //    }
        //
        //    downloadUrl = result.Url;
        //
        //    break;
        //}
        //
        //var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(downloadUrl);
        //
        //var streamInfo = streamManifest.GetAudioOnlyStreams();
        //
        //// choosing best audio stream (comparing their Birate)
        //var bestStream = streamInfo.OrderByDescending(x => x.Bitrate).FirstOrDefault();
        //
        //var savePath = "";
        //
        //await _youtube.Videos.Streams.DownloadAsync(bestStream, savePath);
    }*/

    public async Task DownloadAsync(
        string filePath,
        Track track,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default
    )
    {
        //var downloadUrl = await _spotify.Tracks.GetSpotifymateUrlAsync(track.Url, cancellationToken);

        var videoId = await _spotify.Tracks.GetYoutubeIdAsync(track.Id, cancellationToken);
        var test1 = await y2mate.AnalyzeAsync(videoId!, cancellationToken: cancellationToken);

        var audio = test1.Where(x => x.FileType == FileType.Mp3).FirstOrDefault();
        var downloadUrl = await y2mate.ConvertAsync(
            audio!.Id,
            videoId!,
            cancellationToken: cancellationToken
        );

        //var tt = new WebClient();
        //tt.DownloadFile(downloadUrl, filePath);

        var downloader = new Downloader();
        await downloader.DownloadAsync(
            downloadUrl!,
            filePath,
            null,
            progress?.ToDoubleBased(),
            false,
            cancellationToken
        );
    }
}

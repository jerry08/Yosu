using System;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using Httpz;
using SpotifyExplode;
using SpotifyExplode.Tracks;

namespace Yosu.Spotify.Core.Downloading;

public class TrackDownloader
{
    private readonly SpotifyClient _spotify = new();

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
        var downloadUrl = await _spotify.Tracks.GetDownloadUrlAsync(track.Id, cancellationToken);

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

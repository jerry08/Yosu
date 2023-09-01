using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoundCloudExplode.Tracks;
using Yosu.Soundcloud.Core.Utils;

namespace Yosu.Soundcloud.Core.Tagging;

public class MediaTagInjector
{
    private readonly MusicBrainzClient _musicBrainz = new();

    private static void InjectMiscMetadata(MediaFile mediaFile, Track track)
    {
        if (!string.IsNullOrWhiteSpace(track.Description))
            mediaFile.SetDescription(track.Description);

        mediaFile.SetComment(
            "Downloaded using SoundCloudDownloader (https://github.com/jerry08/SoundCloudDownloader)" +
            Environment.NewLine +
            $"Track: {track.Title}" +
            Environment.NewLine +
            $"Track URL: {track.PermalinkUrl}"
        );
    }

    private async Task InjectMusicMetadataAsync(
        MediaFile mediaFile,
        Track track,
        CancellationToken cancellationToken = default)
    {
        var recordings = await _musicBrainz.SearchRecordingsAsync(track.Title!, cancellationToken);

        var recording = recordings
            .FirstOrDefault(r =>
                // Recording title must be part of the track title.
                // Recording artist must be part of the track title.
                track.Title!.Contains(r.Title, StringComparison.OrdinalIgnoreCase) && (
                    track.Title.Contains(r.Artist, StringComparison.OrdinalIgnoreCase)
                )
            );

        if (recording is null)
            return;

        mediaFile.SetArtist(recording.Artist);
        mediaFile.SetTitle(recording.Title);

        if (!string.IsNullOrWhiteSpace(recording.ArtistSort))
            mediaFile.SetArtistSort(recording.ArtistSort);

        if (!string.IsNullOrWhiteSpace(recording.Album))
            mediaFile.SetAlbum(recording.Album);
    }

    private static void InjectTrackMetadataAsync(
        MediaFile mediaFile,
        Track track)
    {
        mediaFile.SetTitle(track.Title!);
        mediaFile.SetPerformers(new[] { track.User!.Username! });
        mediaFile.SetAlbum(track.User.Username!);
    }

    private async Task InjectThumbnailAsync(
        MediaFile mediaFile,
        Track track,
        CancellationToken cancellationToken = default)
    {
        var url = track.ArtworkUrl?.ToString().Replace("large", "t500x500").Replace("small", "t500x500");

        if (url == null)
        {
            await Task.CompletedTask;
        }

        mediaFile.SetThumbnail(
            await Http.Client.GetByteArrayAsync(url, cancellationToken)
        );
    }

    public async Task InjectTagsAsync(
        string filePath,
        Track track,
        CancellationToken cancellationToken = default)
    {
        using var mediaFile = MediaFile.Create(filePath);

        InjectMiscMetadata(mediaFile, track);
        //await InjectMusicMetadataAsync(mediaFile, track, cancellationToken);
        InjectTrackMetadataAsync(mediaFile, track);
        await InjectThumbnailAsync(mediaFile, track, cancellationToken);
    }
}
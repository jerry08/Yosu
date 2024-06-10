using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SpotifyExplode.Tracks;
using Yosu.Spotify.Core.Utils;

namespace Yosu.Spotify.Core.Tagging;

public class MediaTagInjector
{
    private readonly MusicBrainzClient _musicBrainz = new();

    private void InjectMiscMetadata(MediaFile mediaFile, Track track)
    {
        mediaFile.SetComment(
            "Downloaded using Yosu (https://github.com/jerry08/Yosu)"
                + Environment.NewLine
                + $"Track: {track.Title}"
                + Environment.NewLine
                + $"Track URL: {track.Url}"
                + Environment.NewLine
                + $"Artist: {track.Artists.FirstOrDefault()?.Name}"
        );
    }

    private async Task InjectMusicMetadataAsync(
        MediaFile mediaFile,
        Track track,
        CancellationToken cancellationToken = default
    )
    {
        var recordings = await _musicBrainz.SearchRecordingsAsync(track.Title, cancellationToken);

        var recording = recordings.FirstOrDefault(r =>
            // Recording title must be part of the video title.
            // Recording artist must be part of the video title or channel title.
            track.Title.Contains(r.Title, StringComparison.OrdinalIgnoreCase)
            && (
                track.Title.Contains(r.Artist, StringComparison.OrdinalIgnoreCase)
                || track
                    .Artists.FirstOrDefault()
                    ?.Name.Contains(r.Artist, StringComparison.OrdinalIgnoreCase) == true
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

    private async Task InjectThumbnailAsync(
        MediaFile mediaFile,
        Track track,
        CancellationToken cancellationToken = default
    )
    {
        var thumbnailUrl = track
            .Album.Images?.OrderByDescending(x => x.Height)
            .FirstOrDefault()
            ?.Url;

        mediaFile.SetThumbnail(
            await Http.Client.GetByteArrayAsync(thumbnailUrl, cancellationToken)
        );
    }

    public async Task InjectTagsAsync(
        string filePath,
        Track track,
        CancellationToken cancellationToken = default
    )
    {
        using var mediaFile = MediaFile.Create(filePath);

        InjectMiscMetadata(mediaFile, track);
        await InjectMusicMetadataAsync(mediaFile, track, cancellationToken);
        await InjectThumbnailAsync(mediaFile, track, cancellationToken);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using SpotifyExplode;
using SpotifyExplode.Albums;
using SpotifyExplode.Artists;
using SpotifyExplode.Playlists;
using SpotifyExplode.Tracks;

namespace Yosu.Spotify.Core.Resolving;

public class QueryResolver
{
    private readonly SpotifyClient _spotify = new();

    public async Task<QueryResult> ResolveAsync(
        string query,
        CancellationToken cancellationToken = default
    )
    {
        // Only consider URLs when parsing IDs.
        // All other queries should be treated as search queries.
        var isUrl = Uri.IsWellFormedUriString(query, UriKind.Absolute);

        // Playlist
        if (isUrl && PlaylistId.TryParse(query) is { } playlistId)
        {
            var playlist = await _spotify.Playlists.GetAsync(playlistId, cancellationToken);
            //var tracks = await _spotify.Playlists.GetTracksAsync(playlistId, cancellationToken);
            var tracks = playlist.Tracks;
            return new QueryResult(QueryResultKind.Playlist, $"Playlist: {playlist.Name}", tracks);
        }

        // Album
        if (isUrl && AlbumId.TryParse(query) is { } albumId)
        {
            var album = await _spotify.Albums.GetAsync(albumId, cancellationToken);
            //var tracks = await _spotify.Playlists.GetTracksAsync(playlistId, cancellationToken);
            var tracks = album.Tracks;
            return new QueryResult(QueryResultKind.Album, $"Album: {album.Name}", tracks);
        }

        // Artist
        if (isUrl && ArtistId.TryParse(query) is { } artistId)
        {
            var artist = await _spotify.Artists.GetAsync(artistId, cancellationToken);
            var albums = await _spotify.Artists.GetAllAlbumsAsync(
                artistId,
                null,
                cancellationToken
            );
            var tracks = (
                await Task.WhenAll(
                    albums.Select(async album =>
                        await _spotify.Albums.GetAllTracksAsync(album.Id, cancellationToken)
                    )
                )
            )
                .SelectMany(x => x)
                .ToList();
            return new QueryResult(QueryResultKind.Artist, $"Artist: {artist.Name}", tracks);
        }

        // Track
        if (isUrl && TrackId.TryParse(query) is { } trackId)
        {
            var track = await _spotify.Tracks.GetAsync(trackId, cancellationToken);
            return new QueryResult(QueryResultKind.Track, track.Title, [track]);
        }

        // Search
        {
            var tracks = await _spotify.Search.GetTracksAsync(query, 0, 50, cancellationToken);
            return new QueryResult(QueryResultKind.Search, $"Search: {query}", tracks);
        }
    }

    public async Task<QueryResult> ResolveAsync(
        IReadOnlyList<string> queries,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default
    )
    {
        if (queries.Count == 1)
            return await ResolveAsync(queries.Single(), cancellationToken);

        var tracks = new List<Track>();
        var trackIds = new HashSet<TrackId>();

        var completed = 0;

        foreach (var query in queries)
        {
            var result = await ResolveAsync(query, cancellationToken);

            foreach (var track in result.Tracks)
            {
                if (trackIds.Add(track.Id))
                    tracks.Add(track);
            }

            progress?.Report(Percentage.FromFraction(1.0 * ++completed / queries.Count));
        }

        return new QueryResult(QueryResultKind.Aggregate, $"{queries.Count} queries", tracks);
    }
}

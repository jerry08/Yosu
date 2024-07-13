using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using SoundCloudExplode;
using SoundCloudExplode.Common;
using SoundCloudExplode.Tracks;

namespace Yosu.Soundcloud.Core.Resolving;

public class QueryResolver
{
    private readonly SoundCloudClient _soundcloud = new();

    public async Task<QueryResult> ResolveAsync(
        string query,
        CancellationToken cancellationToken = default
    )
    {
        if (!_soundcloud.IsInitialized)
            await _soundcloud.InitializeAsync(cancellationToken);

        // Playlist/Album
        if (await _soundcloud.Playlists.IsUrlValidAsync(query, cancellationToken))
        {
            var playlist = await _soundcloud.Playlists.GetAsync(query, true, cancellationToken);
            var tracks = await _soundcloud.Playlists.GetTracksAsync(query, cancellationToken);

            foreach (var track in tracks)
                track.ArtworkUrl ??= track.User?.AvatarUrl;

            return new QueryResult(
                QueryResultKind.Playlist,
                $"Playlist: {playlist!.Title}",
                tracks
            );
        }

        // Track
        if (await _soundcloud.Tracks.IsUrlValidAsync(query, cancellationToken))
        {
            var track = await _soundcloud.Tracks.GetAsync(query, cancellationToken);

            track!.ArtworkUrl ??= track.User?.AvatarUrl;

            return new QueryResult(QueryResultKind.Track, track!.Title!, new[] { track });
        }

        // Search
        {
            var tracks = await _soundcloud
                .Search.GetTracksAsync(query, cancellationToken)
                .CollectAsync(100);

            foreach (var track in tracks)
                track.ArtworkUrl ??= track.User?.AvatarUrl;

            return new QueryResult(QueryResultKind.Search, "Tracks", tracks);
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

        var completed = 0;

        foreach (var query in queries)
        {
            var result = await ResolveAsync(query, cancellationToken);

            foreach (var track in result.Tracks)
            {
                track.ArtworkUrl ??= track.User?.AvatarUrl;
                tracks.Add(track);
            }

            progress?.Report(Percentage.FromFraction(1.0 * ++completed / queries.Count));
        }

        return new QueryResult(QueryResultKind.Aggregate, $"{queries.Count} queries", tracks);
    }
}

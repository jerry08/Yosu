using System.Collections.Generic;
using SpotifyExplode.Tracks;

namespace Yosu.Spotify.Core.Resolving;

public record QueryResult(
    QueryResultKind Kind,
    string Title,
    IReadOnlyList<Track> Tracks
);
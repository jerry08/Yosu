using System.Collections.Generic;
using SoundCloudExplode.Tracks;

namespace Yosu.Soundcloud.Core.Resolving;

public record QueryResult(
    QueryResultKind Kind,
    string Title,
    IReadOnlyList<Track> Tracks
);
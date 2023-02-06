using System.Collections.Generic;
using SoundCloudExplode.Track;

namespace Yosu.Soundcloud.Core.Resolving;

public record QueryResult(
    QueryResultKind Kind,
    string Title,
    IReadOnlyList<TrackInformation> Tracks
);
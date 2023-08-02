using System;
using SoundCloudExplode.Tracks;
using Yosu.Core.Utils;

namespace Yosu.Soundcloud.Core.Downloading;

public class FileNameTemplate
{
    public static string Apply(
        string template,
        Track track,
        string ext,
        string? number = null) =>
        PathEx.EscapeFileName(
            template
                .Replace("$num", number is not null ? $"{number}" : "")
                .Replace("$id", $"{track.Id}")
                .Replace("$title", track.Title)
                .Replace("$album", track.PlaylistName)
                //.Replace("$author", track.PublisherMetadata.Artist)
                .Replace("$author", track.User?.Username)
                .Replace("$releasedDate", DateTime.TryParse(track.ReleaseDate?.ToString(), out DateTime releasedDate) ? (releasedDate.ToString("yyyy-MM-dd") ?? "") : "")
                .Trim() + '.' + ext
        );
}
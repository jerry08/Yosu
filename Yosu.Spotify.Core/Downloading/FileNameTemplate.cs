using System.Linq;
using SpotifyExplode.Tracks;
using Yosu.Spotify.Core.Utils;

namespace Yosu.Spotify.Core.Downloading;

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
                .Replace("$album", track.Album.Name)
                .Replace("$trackNumber", $"{track.TrackNumber}")
                .Replace("artists", string.Join(", ", track.Artists.Select(x => x.Name)))
                .Replace("$releasedDate", track.Album.ReleaseDate?.ToString("yyyy-MM-dd") ?? "")
                .Trim() + '.' + ext
        );
}
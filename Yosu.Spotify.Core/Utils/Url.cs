using System.Text.RegularExpressions;
using Yosu.Spotify.Core.Utils.Extensions;

namespace Yosu.Spotify.Core.Utils;

internal static class Url
{
    public static string? TryExtractFileName(string url) =>
        Regex.Match(url, @".+/([^?]*)").Groups[1].Value.NullIfEmptyOrWhiteSpace();
}
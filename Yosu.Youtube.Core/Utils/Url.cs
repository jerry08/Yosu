using System.Text.RegularExpressions;
using Yosu.Youtube.Core.Utils.Extensions;

namespace Yosu.Youtube.Core.Utils;

internal static class Url
{
    public static string? TryExtractFileName(string url) =>
        Regex.Match(url, @".+/([^?]*)").Groups[1].Value.NullIfEmptyOrWhiteSpace();
}
using System.Text.RegularExpressions;
using Yosu.Core.Utils.Extensions;

namespace Yosu.Core.Utils;

public static class Url
{
    public static string? TryExtractFileName(string url) =>
        Regex.Match(url, @".+/([^?]*)").Groups[1].Value.NullIfEmptyOrWhiteSpace();
}

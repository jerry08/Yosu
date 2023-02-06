using System.IO;
using YoutubeExplode.Common;

namespace Yosu.Youtube.Core.Utils.Extensions;

internal static class YoutubeExtensions
{
    public static string? TryGetImageFormat(this Thumbnail thumbnail) =>
        Url.TryExtractFileName(thumbnail.Url)?.Pipe(Path.GetExtension)?.Trim('.');
}
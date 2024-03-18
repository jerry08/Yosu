using System.IO;
using Yosu.Core.Utils;
using Yosu.Core.Utils.Extensions;
using YoutubeExplode.Common;

namespace Yosu.Youtube.Core.Utils.Extensions;

internal static class YoutubeExtensions
{
    public static string? TryGetImageFormat(this Thumbnail thumbnail) =>
        Url.TryExtractFileName(thumbnail.Url)?.Pipe(Path.GetExtension)?.Trim('.');
}

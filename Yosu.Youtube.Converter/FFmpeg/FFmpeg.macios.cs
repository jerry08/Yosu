using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yosu.Youtube.Converter;

internal class FFmpeg(string filePath)
{
    public static string GetFilePath() => string.Empty;

    public async ValueTask ExecuteAsync(
        string arguments,
        IProgress<double>? progress,
        CancellationToken cancellationToken = default
    ) { }
}

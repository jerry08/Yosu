using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yosu.Youtube.Converter;

internal class FFmpeg(string filePath)
{
    private readonly string _filePath = filePath;

    public async ValueTask ExecuteAsync(
        string arguments,
        IProgress<double>? progress,
        CancellationToken cancellationToken = default
    ) { }
}

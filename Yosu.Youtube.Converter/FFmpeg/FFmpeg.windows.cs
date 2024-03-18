using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yosu.Youtube.Converter;

internal class FFmpeg
{
    private readonly string _filePath;

    public FFmpeg(string filePath) => _filePath = filePath;

    public async ValueTask ExecuteAsync(
        string arguments,
        IProgress<double>? progress,
        CancellationToken cancellationToken = default
    ) { }
}

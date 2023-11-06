using System.IO;
using System.Threading.Tasks;

namespace Yosu.Utils.Extensions;

internal static class StreamExtensions
{
    public static async Task<string> ToStringAsync(this Stream stream)
    {
        stream.Position = 0;

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}

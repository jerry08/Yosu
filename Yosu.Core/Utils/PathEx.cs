using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yosu.Core.Utils;

public static class PathEx
{
    private static readonly HashSet<char> InvalidFileNameChars =
        new(Path.GetInvalidFileNameChars());

    public static string EscapeFileName(string path)
    {
        var newPath = path.Trim();

        var buffer = new StringBuilder(newPath.Length);

        foreach (var c in newPath)
            buffer.Append(!InvalidFileNameChars.Contains(c) ? c : '_');

        return buffer.ToString();
    }
}

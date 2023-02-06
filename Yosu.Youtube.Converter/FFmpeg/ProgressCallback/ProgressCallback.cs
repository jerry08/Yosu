using System;
using System.Collections.Generic;

namespace Yosu.Youtube.Converter;

internal partial class ProgressCallback
{
    public const string Tag = "yosu-ffmpeg";

    public long Duration { get; set; }

    public IProgress<double>? Progress { get; set; }

#if !ANDROID
    public static void Init(
        IProgress<double>? progress,
        IEnumerable<string> filePaths)
    {
    }
#endif
}
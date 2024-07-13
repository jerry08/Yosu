using System;
using System.Collections.Generic;
using System.Linq;
using Android.Media;
using Java.IO;
using Microsoft.Maui.ApplicationModel;

namespace Yosu.Youtube.Converter;

internal partial class ProgressCallback
    : Java.Lang.Object,
        Laerdal.FFmpeg.Android.IStatisticsCallback
{
    public void Apply(Laerdal.FFmpeg.Android.Statistics newStatistics)
    {
        Android.Util.Log.Debug(Tag, $"{newStatistics}");

        //var progress = ((double)newStatistics.Time / (double)TotalTime) * 100;
        //Android.Util.Log.Debug("yosu-ffmpeg", $"progress - {progress}");
        Progress?.Report((double)newStatistics.Time / (double)Duration);
    }

    public static void Init(IProgress<double>? progress, string filePath)
    {
        Init(progress, [filePath]);
    }

    public static void Init(IProgress<double>? progress, IEnumerable<string> filePaths)
    {
        //This allows ffmpeg to return result and continue operations in app
        Laerdal.FFmpeg.Android.Config.IgnoreSignal(Laerdal.FFmpeg.Android.Signal.Sigxcpu);

        var durations = new List<long>();

        foreach (var filePath in filePaths)
        {
            var retriever = new MediaMetadataRetriever();
            retriever.SetDataSource(
                Platform.CurrentActivity,
                Android.Net.Uri.FromFile(new File(filePath))
            );
            var time = retriever.ExtractMetadata(MetadataKey.Duration) ?? string.Empty;
            durations.Add(long.Parse(time));
        }

        // Get highest duration to compare with ffmpeg progress time
        durations = durations.OrderByDescending(d => d).ToList();

        Laerdal.FFmpeg.Android.Config.EnableStatisticsCallback(
            new ProgressCallback() { Progress = progress, Duration = durations.FirstOrDefault() }
        );
    }
}

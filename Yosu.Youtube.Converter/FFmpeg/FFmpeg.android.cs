using System;
using System.Threading;
using System.Threading.Tasks;
using Yosu.Youtube.Converter.Utils.Extensions;

namespace Yosu.Youtube.Converter;

internal class FFmpeg(string filePath)
{
    public static string GetFilePath() => string.Empty;

    public async ValueTask ExecuteAsync(
        string arguments,
        IProgress<double>? progress,
        CancellationToken cancellationToken = default
    )
    {
        //This allows ffmpeg to return result and continue operations in app
        //Laerdal.FFmpeg.Android.Config.IgnoreSignal(Laerdal.FFmpeg.Android.Signal.Sigxcpu);

        //Laerdal.FFmpeg.Android.Config.EnableStatisticsCallback(new Test2(progress));

        var isBusy = true;

        Task.Run(
                async () =>
                {
                    while (isBusy)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Laerdal.FFmpeg.Android.FFmpeg.Cancel();
                            break;
                        }

                        //cancellationToken.ThrowIfCancellationRequested();

                        await Task.Delay(500);
                    }
                },
                cancellationToken
            )
            .FireAndForget();

        var exitCode = Laerdal.FFmpeg.Android.FFmpeg.Execute(arguments);
        //var exitCode = Laerdal.FFmpeg.Android.FFmpeg.ExecuteAsync(arguments, new Test1(progress));

        isBusy = false;

        if (exitCode == Laerdal.FFmpeg.Android.Config.ReturnCodeCancel)
        {
            throw new OperationCanceledException();
        }
        else if (exitCode != Laerdal.FFmpeg.Android.Config.ReturnCodeSuccess)
        {
            throw new InvalidOperationException(
                $"""
                FFmpeg exited with a non-zero exit code ({exitCode}).

                Arguments:
                {arguments}
                """
            );
        }
    }
}

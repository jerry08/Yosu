using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using AndroidX.Lifecycle;
using Yosu.Utils;
using Yosu.ViewModels;

namespace Yosu.Services;

//[Service(Exported = true, Enabled = true, Name = "com.berry.yosuservice", Process = ":yosudownloader")]
[Service(Exported = true, Enabled = true, Name = "com.berry.yosuservice")]
public class ForegroundService : LifecycleService
{
    public const string Tag = "Foreground Service";

    public PowerManager.WakeLock? WakeLock { get; set; }

    public static bool IsServiceStarted { get; set; }

    class DownloadServiceBinder : Binder
    {
        public DownloadServiceBinder() { }
    }

    IBinder myBinder = new DownloadServiceBinder();

    public override IBinder? OnBind(Intent intent)
    {
        base.OnBind(intent);
        return myBinder;
    }

    public override void OnCreate()
    {
        base.OnCreate();

        NotificationHelper.ShowNotification(this, "Downloading", "Downloading...");
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(
        Intent? intent,
        [GeneratedEnum] StartCommandFlags flags,
        int startId
    )
    {
        base.OnStartCommand(intent, flags, startId);

        // Send a notification that service is started
        //SendNotification(this, "Downloading", "Downloading...");

        // Send a notification that service is started
        Log.Info(Tag, "Foreground Service Started.");

        if (intent?.Action == "kill")
            KillService();

        // Wake locks and misc tasks from here :
        if (IsServiceStarted)
        {
            // Service Already Started
            return StartCommandResult.Sticky;
        }
        else
        {
            IsServiceStarted = true;

            Log.Info(Tag, "Starting the foreground service task");

            WakeLock = PowerManager
                .FromContext(this)
                ?.NewWakeLock(WakeLockFlags.Partial, "EndlessService::lock");
            WakeLock?.Acquire();

            Log.Info(Tag, "Started the foreground service task");

            StartScan();

            return StartCommandResult.Sticky;
        }
    }

    private void StartScan()
    {
        Task.Run(async () =>
        {
            while (
                YoutubeViewModel.Downloads.Count > 0
                || SoundcloudViewModel.Downloads.Count > 0
                || SpotifyViewModel.Downloads.Count > 0
            )
            {
                await Task.Delay(3000);
            }

            if (IsServiceStarted)
                KillService();
        });
    }

    public override void OnTaskRemoved(Intent? rootIntent)
    {
        base.OnTaskRemoved(rootIntent);

        Log.Info(Tag, "Task removed");
    }

    private void ReleaseWakeLock()
    {
        Log.Debug(Tag, "Releasing Wake Lock");

        try
        {
            if (WakeLock?.IsHeld == true)
                WakeLock?.Release();
        }
        catch (Exception e)
        {
            Log.Debug(Tag, $"Service stopped without being started: {e.Message}");
        }

        IsServiceStarted = false;
    }

    [SuppressMessage(
        "Interoperability",
        "CA1422:Validate platform compatibility",
        Justification = "<Pending>"
    )]
    private void KillService()
    {
        // Crashing and closing app
        //try
        //{
        //    YoutubeViewModel.Downloads.ForEach(download => download.Cancel());
        //    FFmpeg.Cancel();
        //}
        //catch { }

        try
        {
            YoutubeViewModel.Downloads.ForEach(download => download.Cancel());
            SoundcloudViewModel.Downloads.ForEach(download => download.Cancel());
            SpotifyViewModel.Downloads.ForEach(download => download.Cancel());
        }
        catch { }

        ReleaseWakeLock();

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            StopForeground(true);
            StopSelf();
        }
        else
        {
            StopSelf();
        }
    }
}

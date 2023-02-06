using Microsoft.Maui.Platform;
using Microsoft.Maui.ApplicationModel;
using Android.App;
using AndroidX.Lifecycle;
using Android.OS;
using Android.Runtime;
using Android.Content;
using Android.Util;
using AndroidX.Core.App;
using System.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using Yosu.ViewModels;
using Laerdal.FFmpeg.Android;

namespace Yosu.Platforms.Services;

//[Service(Exported = true, Enabled = true, Name = "com.berry.yosuservice", Process = ":yosudownloader")]
[Service(Exported = true, Enabled = true, Name = "com.berry.yosuservice")]
public class ForegroundService : LifecycleService
{
    public const string Tag = "Foreground Service";
    public const string ChannelId = "ForegroundDownloaderService";
    public const int NotificationId = 2781;

    public PowerManager.WakeLock? WakeLock { get; set; }

    public static bool IsServiceStarted { get; set; }

    class DownloadServiceBinder : Binder
    {
        public DownloadServiceBinder()
        {
        }
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

        SendNotification(this, "Downloading", "Downloading...");
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        base.OnStartCommand(intent, flags, startId);

        // Send a notification that service is started
        //SendNotification(this, "Downloading", "Downloading...");

        // Send a notification that service is started
        Log.Info(Tag, "Foreground Service Started.");

        if (intent?.Action == "kill")
            KillService();

        //ForegroundService.SendNotification(this, 2872, "test", "test");
        //Toast.Make("Foreground Service Started", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();

        //StartForeground(NotificationId, );
        //SendNotification(this, "Downloading", "Downloading...");

        //Task.Run(async () =>
        //{
        //    for (int i = 0; i < 20; i++)
        //    {
        //        await Task.Delay(1000);
        //        SendNotification(this, $"test{i}", $"test{i}");
        //        //Toast.Make("ss", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        //    }
        //});

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

            WakeLock = PowerManager.FromContext(this)?
                .NewWakeLock(WakeLockFlags.Partial, "EndlessService::lock");
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
            while (YoutubeViewModel.Downloads.Count > 0
                || SoundcloudViewModel.Downloads.Count > 0
                || SpotifyViewModel.Downloads.Count > 0)
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

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility", Justification = "<Pending>")]
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

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static void SendNotification(
        Service service,
        string textTitle,
        string textContent)
    {
        var intent = new Intent(service, typeof(MainActivity));
        var pendingIntent = PendingIntent.GetActivity(
            service,
            0,
            intent,
            PendingIntentFlags.Immutable
        );

        var cancelIntent = new Intent(service, typeof(ForegroundService));
        cancelIntent.SetAction("kill");
        var cancelPendingIntent = PendingIntent.GetForegroundService(
            service,
            3462,
            cancelIntent,
            PendingIntentFlags.UpdateCurrent
        );

        var channelId = $"{service.PackageName}.general";

        var builder = new NotificationCompat.Builder(service, channelId)
            .SetSmallIcon(Resource.Drawable.logo_notification)
            .SetOngoing(true)
            .SetContentTitle(textTitle)
            .SetContentText(textContent)
            .SetContentIntent(pendingIntent)
            .SetProgress(100, 100, true)
            .AddAction(Resource.Drawable.logo_notification, "Cancel All", cancelPendingIntent)
            //.SetPriority(NotificationCompat.PriorityDefault);
            .SetPriority(NotificationCompat.PriorityLow);

        service.StartForeground(NotificationId, builder.Build());
    }

    public static void SendNotification(
        Context service,
        int notificationId,
        string textTitle,
        string textContent)
    {
        var intent = new Intent(service, typeof(MainActivity));
        var pendingIntent = PendingIntent.GetActivity(service, 0, intent, PendingIntentFlags.Immutable);

        var channelId = $"{service.PackageName}.general";

        //Application.Context.Resources?.GetIdentifier();

        var builder = new NotificationCompat.Builder(service, channelId)
            .SetSmallIcon(Resource.Drawable.logo_notification)
            .SetOngoing(true)
            .SetContentTitle(textTitle)
            .SetContentText(textContent)
            .SetContentIntent(pendingIntent)
            .SetPriority(NotificationCompat.PriorityLow);

        var notificationManager = NotificationManagerCompat.From(service);
        //var notificationManager = (NotificationManager)GetSystemService(NotificationService);

        notificationManager.Notify(notificationId, builder.Build());
    }
}
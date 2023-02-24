using System.Diagnostics.CodeAnalysis;
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Microsoft.Maui.ApplicationModel;
using Yosu.Services;

namespace Yosu.Utils;

public static class NotificationHelper
{
    public const string ChannelId = "ForegroundDownloaderService";
    public const int NotificationId = 2781;
    public const int CompletedNotificationId = 2682;
    public const int CancelIntentRequestCode = 3462;

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static void ShowNotification(
        Service service,
        string textTitle,
        string textContent)
    {
        var flags = PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable;

        var intent = new Intent(service, typeof(MainActivity));
        var pendingIntent = PendingIntent.GetActivity(service, 0, intent, flags);

        var cancelIntent = new Intent(service, typeof(ForegroundService));
        cancelIntent.SetAction("kill");
        var cancelPendingIntent = PendingIntent.GetForegroundService(service, CancelIntentRequestCode, cancelIntent, flags);

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

    public static void ShowCompletedNotification()
    {
        var context = Platform.AppContext;

        var channelId = $"{context.PackageName}.general";

        //Application.Context.Resources?.GetIdentifier();

        var builder = new NotificationCompat.Builder(context, channelId)
            .SetSmallIcon(Resource.Drawable.logo_notification)
            //.SetOngoing(true)
            .SetContentTitle("Yosu")
            .SetContentText("Complete")
            .SetPriority(NotificationCompat.PriorityLow);

        var notificationManager = NotificationManagerCompat.From(context);
        //var notificationManager = (NotificationManager)GetSystemService(NotificationService);

        notificationManager.Notify(CompletedNotificationId, builder.Build());
    }
}
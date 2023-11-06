using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Service.Notification;

namespace Yosu.Utils;

public static class ApplicationEx
{
    /// <summary>
    /// Checks if the app it fully visible (active) and running. (Foreground)
    /// </summary>
    public static bool IsInForeground()
    {
        var appProcessInfo = new ActivityManager.RunningAppProcessInfo();
        ActivityManager.GetMyMemoryState(appProcessInfo);
        return appProcessInfo.Importance == Importance.Foreground
            || appProcessInfo.Importance == Importance.Visible;
    }

    public static bool IsInBackground()
    {
        var appProcessInfo = new ActivityManager.RunningAppProcessInfo();
        ActivityManager.GetMyMemoryState(appProcessInfo);
        return appProcessInfo.Importance == Importance.Background;
    }

    public static bool IsRunning()
    {
        var appProcessInfo = new ActivityManager.RunningAppProcessInfo();
        ActivityManager.GetMyMemoryState(appProcessInfo);
        return appProcessInfo.Importance == Importance.Foreground
            //|| appProcessInfo.Importance == Importance.ForegroundService
            || appProcessInfo.Importance == Importance.Visible;
    }

    public static Notification? GetActiveNotification(Context? context, int id) =>
        GetActiveStatusBarNotification(context, id)?.Notification;

    public static StatusBarNotification? GetActiveStatusBarNotification(Context? context, int id) =>
        GetActiveStatusBarNotifications(context)?.Where(x => x.Id == id).FirstOrDefault();

    public static Notification[] GetActiveNotifications(Context? context) =>
        GetActiveStatusBarNotifications(context)
            .Where(x => x is not null)
            .Select(x => x.Notification!)
            .ToArray() ?? Array.Empty<Notification>();

    public static StatusBarNotification[] GetActiveStatusBarNotifications(Context? context)
    {
        var emptyArray = Array.Empty<StatusBarNotification>();

        if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            return emptyArray;

        if (context is null)
            return emptyArray;

        var notificationManager = NotificationManager.FromContext(context);
        if (notificationManager is null)
            return emptyArray;

#pragma warning disable CA1416
        var barNotifications = notificationManager.GetActiveNotifications();
#pragma warning restore CA1416

        return barNotifications?.ToArray() ?? emptyArray;
    }
}

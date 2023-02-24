using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui;
using Yosu.ViewModels;

namespace Yosu;

[IntentFilter(
    actions: new[] { Intent.ActionView },
    Label = "Download in Yosu",
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataSchemes = new[] { "http", "https" },
    DataHosts = new[] { "youtube.com", "youtu.be", "on.soundcloud.com" },
    DataPathPatterns = new[] { "/.*" }
)]
[IntentFilter(
    actions: new[] { Intent.ActionView },
    Label = "Download in Yosu",
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataSchemes = new[] { "http", "https" },
    DataHosts = new[] { "soundcloud.com", "www.soundcloud.com", "m.soundcloud.com" },
    DataPathPatterns = new[] { "/.*/.*" }
)]
//[IntentFilter(
//    actions: new[] { Intent.ActionSend },
//    Label = "Download in Yosu",
//    Categories = new[] { Intent.CategoryDefault },
//    DataMimeTypes = new[] { "text/plain" }
//)]
[Activity(Exported = true, Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const int PostNotificationsRequestCode = 1006;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        //AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

        base.OnCreate(savedInstanceState);

        UserDialogs.Init(this);

        if (Build.VERSION.SdkInt > BuildVersionCodes.S)
        {
            if (ContextCompat.CheckSelfPermission(this,
                Manifest.Permission.PostNotifications)
                != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this,
                    new string[] { Manifest.Permission.PostNotifications },
                    PostNotificationsRequestCode);
            }
        }

        CreateNotificationChannelIfNeeded();

        if (Intent?.Data is Android.Net.Uri uri)
        {
            MainCollectionViewModel.IntentUrl = uri.ToString();
        }
    }

    private void CreateNotificationChannelIfNeeded()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateNotificationChannel();
        }
    }

    private void CreateNotificationChannel()
    {
        var channelId = $"{PackageName}.general";
        var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
        var channel = new NotificationChannel(channelId, "General", NotificationImportance.Low);
        notificationManager.CreateNotificationChannel(channel);
    }
}
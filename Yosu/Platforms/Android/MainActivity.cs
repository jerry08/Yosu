using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui;
using Yosu.Utils;

namespace Yosu;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
//[Activity(MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public AndroidStoragePermission? AndroidStoragePermission;

    static int PostNotificationsRequestCode = 1006;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        //AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

        base.OnCreate(savedInstanceState);

        UserDialogs.Init(this);

        //AppCompatDelegate.DefaultNightMode = (int)UiNightMode.Yes;
        //AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;

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
        notificationManager?.CreateNotificationChannel(channel);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        AndroidStoragePermission?.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        AndroidStoragePermission?.OnActivityResult(requestCode, resultCode, data);
    }
}
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Webkit;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.DocumentFile.Provider;
using Java.Nio.FileNio.Attributes;
using Microsoft.Maui;
using Microsoft.Maui.Storage;
using Yosu.Extensions;
using Yosu.Platforms.Android;
using Yosu.Utils;
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
    private const int TakeCardUriPermissionRequestCode = 1007;

    protected override async void OnCreate(Bundle? savedInstanceState)
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

        // Migration
        var data1 = await SecureStorage.GetAsync("PreferenceService");
        if (!string.IsNullOrEmpty(data1))
        {
            System.IO.File.WriteAllText(System.IO.Path.Combine(FileSystem.AppDataDirectory, "Preferences.json"), data1);
            SecureStorage.Remove("PreferenceService");
        }

        var data2 = await SecureStorage.GetAsync("SettingsService");
        if (!string.IsNullOrEmpty(data2))
        {
            System.IO.File.WriteAllText(System.IO.Path.Combine(FileSystem.AppDataDirectory, "Settings.json"), data2);
            SecureStorage.Remove("SettingsService");
        }

        //var test1 = ContextCompat.GetExternalFilesDirs(this, null);
        //
        //var tt1 = FileEx.IsExternalStorageAvailable();
        //var tt2 = FileEx.IsExternalStorageReadOnly();
        //var tt3 = Android.OS.Environment.ExternalStorageDirectory;
        //var tt32 = Android.OS.Environment.ExternalStorageDirectory.Path;
        //var tt4 = Android.OS.Environment.ExternalStorageState;
        //var t6 = new Java.IO.File("/" + tt4);
        //var gg = t6.List();
        //var t7 = new Java.IO.File("/mnt");
        //var gg2 = t7.List();
        //var tt5 = Android.OS.Environment.GetExternalStorageState(null);

        /*var pp = AndroidX.Core.Content.ContextCompat.GetExternalFilesDirs(this, null);
        var pp2 = pp.LastOrDefault().ParentFile.ParentFile.ParentFile.ParentFile;

        var uu = Android.Net.Uri.FromFile(pp.LastOrDefault());
        var uri2 = this.ContentResolver?.Insert(uu, null);

        var ggsv = Java.IO.File.CreateTempFile("test24.txt", null, pp2);

        var oldP = System.IO.Path.Combine(FileSystem.CacheDirectory, "mytest.txt");
        var newP = System.IO.Path.Combine(pp2.AbsolutePath, "mytest.txt");
        System.IO.File.WriteAllText(oldP, "test");

        await this.CopyFileAsync(oldP, newP);*/

        //var pps = this.GetPersistedUriPermissions();
        //var pp1 = System.IO.Path.Combine(
        //            Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath,
        //            "Yosu"
        //        );
        //var u2 = Android.Net.Uri.Parse("");
        //var doc = DocumentFile.FromTreeUri(this, u2);

        //var pps = this.GetPersistedUriPermissions().FirstOrDefault();
        //
        //var mm = MimeTypeMap.Singleton.GetMimeTypeFromExtension("txt");
        //var documentDir = DocumentFile.FromTreeUri(this, pps.Uri);
        //var ss = documentDir.ListFiles().ToList();
        //var f1 = documentDir.FindFile("【July】残響散歌 (Zankyou Sanka_ Zankyosanka) Demon Slayer S3 OP _ Aimer【歌ってみた _ COVER】");
        //var test1 = documentDir.CreateFile(mm, "【July】残響散歌 (Zankyou Sanka/ Zankyosanka) Demon Slayer S3 OP / Aimer【歌ってみた / COVER】");
        //
        //var sc = documentDir.FindFile("【July】残響散歌 (Zankyou Sanka/ Zankyosanka) Demon Slayer S3 OP / Aimer【歌ってみた / COVER】");
    }

    private void CreateNotificationChannelIfNeeded()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateNotificationChannel();
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    private void CreateNotificationChannel()
    {
        var channelId = $"{PackageName}.general";
        var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
        var channel = new NotificationChannel(channelId, "General", NotificationImportance.Low);
        notificationManager?.CreateNotificationChannel(channel);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode == TakeCardUriPermissionRequestCode)
        {
            try
            {
                if (data?.Data is not null)
                {
                    var flags = data.Flags & (ActivityFlags.GrantWriteUriPermission |
                        ActivityFlags.GrantReadUriPermission);

                    var uri = data.Data;

                    //GrantUriPermission(
                    //    PackageName,
                    //    uri,
                    //    flags
                    //);

                    ContentResolver!.TakePersistableUriPermission(uri, flags);
                    PickDirectoryPermissionResult?.TrySetResult(new(resultCode, data));

                    //var pps = this.GetPersistedUriPermissions();

                    //var mm = MimeTypeMap.Singleton.GetMimeTypeFromExtension("txt");
                    //var documentDir = DocumentFile.FromTreeUri(this, uri);
                    //var test1 = documentDir.CreateFile(mm, "test3");
                    //
                    //var gsv = test1.CanWrite();
                    //
                    //var sva = ContentResolver.OpenOutputStream(test1.Uri);
                    //var aa = new byte[] { 23, 3 };
                    //sva.Write(aa, 0, aa.Length);
                    //
                    //var ff = new Java.IO.File(test1.Uri.Path);
                    //var hh = ff.CanWrite();
                }
                else
                {
                    PickDirectoryPermissionResult?.TrySetResult(new());
                }
            }
            catch (System.Exception e)
            {
                PickDirectoryPermissionResult?.TrySetResult(new());
            }
        }
    }

    private TaskCompletionSource<PickDirectoryResult>? PickDirectoryPermissionResult;
    public Task<PickDirectoryResult> PickDirectoryAsync()
    {
        PickDirectoryPermissionResult ??= new();

        try
        {
            StartActivityForResult(
                new Intent(Intent.ActionOpenDocumentTree),
                TakeCardUriPermissionRequestCode
            );
        }
        catch
        {
            return Task.FromResult(new PickDirectoryResult());
        }

        return PickDirectoryPermissionResult.Task;
    }
}
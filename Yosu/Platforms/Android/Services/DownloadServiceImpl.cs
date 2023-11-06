using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Webkit;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel;
using Yosu.Utils;

namespace Yosu.Services;

public class DownloadServiceImpl : IDownloadService
{
    public async Task EnqueueAsync(
        string fileName,
        string url,
        Dictionary<string, string>? headers = null
    )
    {
        var status = await StoragePermissionUtil.CheckAndRequestStoragePermission();
        if (status == PermissionStatus.Denied)
        {
            await Snackbar.Make("Storage permission not granted.").Show();
            return;
        }

        var extension = System.IO.Path.GetExtension(fileName).Split('.').LastOrDefault();

        var mime = MimeTypeMap.Singleton!;
        var mimeType = mime.GetMimeTypeFromExtension(extension);

        //string invalidCharRemoved = Episode.EpisodeName.Replace("[\\\\/:*?\"<>|]", "");

        var invalidChars = System.IO.Path.GetInvalidFileNameChars();

        var invalidCharsRemoved = new string(
            fileName.Where(x => !invalidChars.Contains(x)).ToArray()
        );

        var request = new DownloadManager.Request(Android.Net.Uri.Parse(url));

        for (var i = 0; i < headers?.Count; i++)
            request.AddRequestHeader(headers.ElementAt(i).Key, headers.ElementAt(i).Value);

        request.SetMimeType(mimeType);
        request.AllowScanningByMediaScanner();
        request.SetNotificationVisibility(DownloadVisibility.VisibleNotifyCompleted);
        //request.SetDestinationInExternalFilesDir(mainactivity.ApplicationContext, pathToMyFolder, songFullName + ".mp3");
        //request.SetDestinationInExternalPublicDir(Android.OS.Environment.DirectoryMusic, songFullName + ".mp3");

        //request.SetDestinationInExternalPublicDir(WeebUtils.AppFolderName, invalidCharsRemoved + ".mp4");
        request.SetDestinationInExternalPublicDir(
            Android.OS.Environment.DirectoryDownloads,
            invalidCharsRemoved
        );
        var dm = (DownloadManager)
            Application.Context.GetSystemService(Android.Content.Context.DownloadService)!;
        var id = dm.Enqueue(request);
    }
}

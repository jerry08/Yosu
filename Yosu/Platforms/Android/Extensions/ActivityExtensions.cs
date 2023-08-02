﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Webkit;
using AndroidX.DocumentFile.Provider;

namespace Yosu.Extensions;

internal static class ActivityExtensions
{
    public static List<UriPermission> GetPersistedUriPermissions(this Context context)
        => context.ContentResolver?.PersistedUriPermissions.ToList() ?? new();

    public static bool HasPersistedUriPermission(this Context context, string uri)
        => GetPersistedUriPermissions(context).Any(x => x.Uri?.Path == uri);

    public static Android.Net.Uri? GetPersistedUriPermissionFor(this Context context, string uri)
        => GetPersistedUriPermissions(context).Find(x => x.Uri?.Path == uri)?.Uri;

    public static async Task CopyFileAsync(
        this Context context,
        string filePath,
        string newFilePath,
        CancellationToken cancellationToken = default)
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
            await CopyFileUsingMediaStoreAsync(context, filePath, newFilePath, cancellationToken);
        }
        else
        {
            File.Copy(filePath, newFilePath, true);

            var ext = Path.GetExtension(newFilePath).Replace(".", "");
            var mime = MimeTypeMap.Singleton!;
            var mimeType = mime.GetMimeTypeFromExtension(ext);

            if (!string.IsNullOrEmpty(mimeType))
            {
                MediaScannerConnection.ScanFile(
                    context,
                    new[] { newFilePath },
                    new[] { mimeType },
                    null
                );
            }
        }
    }

    private static async Task CopyFileUsingMediaStoreAsync(
        this Context context,
        string filePath,
        string newFilePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            return;

        var ext = Path.GetExtension(newFilePath).Replace(".", "");
        //var dir = Directory.GetParent(newFilePath)?.FullName;
        var fileName = Path.GetFileNameWithoutExtension(newFilePath);

        var mimeType = MimeTypeMap.Singleton!.GetMimeTypeFromExtension(ext);

        if (string.IsNullOrEmpty(mimeType))
            return;

        //var dir = Android.OS.Environment.DirectoryDownloads;
        //var dir = "Yosu";
        var dir = Path.GetDirectoryName(newFilePath)!
            .Replace(Environment.ExternalStorageDirectory!.AbsolutePath, "");
        if (dir.StartsWith("/"))
            dir = dir[1..];

        var hasPersistedUriPermission = context.HasPersistedUriPermission(
            Path.GetDirectoryName(newFilePath)!
        );

        var uri = context.GetPersistedUriPermissionFor(Path.GetDirectoryName(newFilePath)!);

        if (hasPersistedUriPermission)
        {
            if (uri is null)
                return;

            //var uri2 = Android.Net.Uri.Parse(Path.Combine(uri.Path!, Path.GetFileName(newFilePath)));

            //var test1 = DocumentFile.IsDocumentUri(context, uri);
            //var test2 = DocumentFile.FromSingleUri(context, uri);
            var test2 = DocumentFile.FromTreeUri(context, uri);

            var ff1 = test2.FindFile(Path.GetFileName(newFilePath));
            if (ff1 is not null && ff1.Exists())
            {
                ff1.Delete();
            }

            //var gs = test2.Exists();
            //if (test2.Exists())
            //    test2.Delete();

            var newFile = test2.CreateFile(mimeType, fileName);
            uri = newFile.Uri;
        }
        else
        {
            var contentValues = new ContentValues();
            //contentValues.Put(MediaStore.IMediaColumns.DisplayName, newFilePath);
            contentValues.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            contentValues.Put(MediaStore.IMediaColumns.MimeType, mimeType);
            contentValues.Put(MediaStore.IMediaColumns.RelativePath, dir);
            //contentValues.Put(MediaStore.IMediaColumns.RelativePath, dir);
            contentValues.Put(MediaStore.IMediaColumns.Size, new FileInfo(filePath).Length);

            if (mimeType.StartsWith("image") || mimeType.StartsWith("video"))
            {
                // Set media duration
                var retriever = new MediaMetadataRetriever();
                retriever.SetDataSource(context, Android.Net.Uri.FromFile(new Java.IO.File(filePath)));
                var time = retriever.ExtractMetadata(MetadataKey.Duration) ?? string.Empty;
                var timeInMillisec = long.Parse(time);
                contentValues.Put(MediaStore.IMediaColumns.Duration, timeInMillisec);
            }

            var externalContentUri = MediaStore.Files.GetContentUri("external")!;

            //uri = context.ContentResolver?.Insert(MediaStore.Downloads.ExternalContentUri, contentValues);
            uri = context.ContentResolver?.Insert(externalContentUri, contentValues);
        }

        if (uri is null)
            return;

        var defaultBufferSize = 4096;

        using var input = File.OpenRead(filePath);
        var output = context.ContentResolver?.OpenOutputStream(uri);
        if (output is not null)
            await input.CopyToAsync(output, defaultBufferSize, cancellationToken);

        MediaScannerConnection.ScanFile(
            context,
            new[] { newFilePath },
            new[] { mimeType },
            null
        );
    }
}
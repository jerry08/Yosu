using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Media;
using Android.Webkit;
using Android.Content;
using Android.Provider;

namespace Yosu.Extensions;

internal static class ActivityExtensions
{
    public static async Task CopyFileUsingMediaStore(
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

        var mime = MimeTypeMap.Singleton!;
        var mimeType = mime.GetMimeTypeFromExtension(ext);

        if (mimeType is null)
            return;

        var fileInfo = new FileInfo(filePath);

        var contentValues = new ContentValues();
        //contentValues.Put(MediaStore.IMediaColumns.DisplayName, newFilePath);
        contentValues.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        contentValues.Put(MediaStore.IMediaColumns.MimeType, mimeType);
        contentValues.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);
        //contentValues.Put(MediaStore.IMediaColumns.RelativePath, dir);
        contentValues.Put(MediaStore.IMediaColumns.Size, fileInfo.Length);

        if (mimeType.StartsWith("image") || mimeType.StartsWith("video"))
        {
            //Set media duration
            var retriever = new MediaMetadataRetriever();
            retriever.SetDataSource(context, Android.Net.Uri.FromFile(new Java.IO.File(filePath)));
            var time = retriever.ExtractMetadata(MetadataKey.Duration) ?? string.Empty;
            var timeInMillisec = long.Parse(time);
            contentValues.Put(MediaStore.IMediaColumns.Duration, timeInMillisec);
        }

        var resolver = context.ContentResolver;
        var externalContentUri = MediaStore.Files.GetContentUri("external")!;

        //var uri = resolver?.Insert(MediaStore.Downloads.ExternalContentUri, contentValues);
        var uri = resolver?.Insert(externalContentUri, contentValues);
        if (uri is not null)
        {
            var defaultBufferSize = 4096;

            using var input = File.OpenRead(filePath);
            var output = resolver?.OpenOutputStream(uri);
            if (output is not null)
                await input.CopyToAsync(output, defaultBufferSize, cancellationToken);
        }

        MediaScannerConnection.ScanFile(
            context,
            new[] { newFilePath },
            new[] { mimeType },
            null
        );
    }
}
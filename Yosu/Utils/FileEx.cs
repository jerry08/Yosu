using System.IO;
using Microsoft.Maui.ApplicationModel;
using Yosu.Utils.Extensions;
#if ANDROID
using Android.OS;
using AndroidX.DocumentFile.Provider;
#endif

namespace Yosu.Utils;

public static class FileEx
{
#if ANDROID
    public static bool IsExternalStorageAvailable() =>
        Environment.ExternalStorageState == Environment.MediaMounted;

    public static bool IsExternalStorageReadOnly() =>
        Environment.ExternalStorageState == Environment.MediaMountedReadOnly;

    public static bool Exists(string filePath)
    {
        var uri = Platform.AppContext.GetPersistedUriPermissionFor(
            Path.GetDirectoryName(filePath)!
        );
        if (uri is not null)
        {
            var documentFile = DocumentFile.FromTreeUri(Platform.AppContext, uri);
            return documentFile?.FindFile(Path.GetFileName(filePath)) is not null
                || File.Exists(filePath);
        }

        return File.Exists(filePath);
    }
#else
    public static bool Exists(string filePath) => File.Exists(filePath);
#endif
}

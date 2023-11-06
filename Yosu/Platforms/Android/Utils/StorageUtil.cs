using System;
using System.Collections.Generic;
using Android.Content;
using AndroidX.DocumentFile.Provider;
using Java.IO;
using Environment = Android.OS.Environment;

namespace Yosu.Utils;

public static class StorageUtil
{
    public static List<File> GetRemovableStorageRoots(this Context context)
    {
        var roots = context.GetExternalFilesDirs("external");
        var rootsArrayList = new List<File>();

        for (int i = 0; i < roots?.Length; i++)
        {
            if (roots[i] != null)
            {
                var path = roots[i].Path;
                int index = path.LastIndexOf("/Android/data/");
                if (index > 0)
                {
                    path = path.Substring(0, index);
                    if (!path.Equals(Environment.ExternalStorageDirectory?.Path))
                    {
                        rootsArrayList.Add(new File(path));
                    }
                }
            }
        }

        return rootsArrayList;
    }

    public static string? GetSdCardRootPath(this Context context, string path)
    {
        var roots = GetRemovableStorageRoots(context);
        for (int i = 0; i < roots.Count; i++)
        {
            if (path.StartsWith(roots[i].Path))
            {
                return roots[i].Path;
            }
        }
        return null;
    }

    public static DocumentFile? ParseDocumentFile(
        this Context context,
        Android.Net.Uri treeUri,
        File file
    )
    {
        DocumentFile? treeRoot;
        try
        {
            treeRoot = DocumentFile.FromTreeUri(context, treeUri);
        }
        catch (Exception e)
        {
            return null;
        }

        string path;
        try
        {
            path = file.CanonicalPath;
            var sdCardPath = GetSdCardRootPath(context, path);
            if (sdCardPath != null)
            {
                if (sdCardPath.Equals(path))
                {
                    return treeRoot;
                }
                path = path.Substring(sdCardPath.Length + 1);
            }
            else
            {
                return null;
            }
        }
        catch (IOException e)
        {
            return null;
        }

        if (treeRoot != null)
        {
            treeRoot = DocumentFile.FromTreeUri(context, treeUri);
            var pathParts = path.Split("/");
            var documentFile = treeRoot;
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (documentFile != null)
                {
                    documentFile = documentFile.FindFile(pathParts[i]);
                }
                else
                {
                    return null;
                }
            }

            return documentFile;
        }

        return null;
    }

    public static DocumentFile? CreateDocumentFile(
        this Context context,
        Android.Net.Uri treeUri,
        string path,
        string mimeType
    )
    {
        var index = path.LastIndexOf("/");
        var dirPath = path.Substring(0, index);
        var file = ParseDocumentFile(context, treeUri, new File(dirPath));
        if (file != null)
        {
            var name = path.Substring(index + 1);
            file = file.CreateFile(mimeType, name);
        }
        return file;
    }

    public static DocumentFile? CreateDocumentDir(
        this Context context,
        Android.Net.Uri treeUri,
        string path
    )
    {
        var index = path.LastIndexOf("/");
        var dirPath = path.Substring(0, index);
        var file = ParseDocumentFile(context, treeUri, new File(dirPath));
        if (file != null)
        {
            var name = path.Substring(index + 1);
            file = file.CreateDirectory(name);
        }
        return file;
    }
}

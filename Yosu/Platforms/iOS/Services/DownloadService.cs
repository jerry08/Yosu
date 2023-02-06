using Foundation;
using Yosu.Services;
using System.Collections.Specialized;

namespace Yosu.Platforms.Services;

public class DownloadService : IDownloadService
{
    public static DownloadService Create() => new();

    public void Download(
        string fileName,
        string url,
        NameValueCollection? headers = null)
    {
        var configuration = NSUrlSessionConfiguration
            .CreateBackgroundSessionConfiguration("com.SimpleBackgroundTransfer.BackgroundSession");
        var session = NSUrlSession.FromConfiguration
            (configuration, new MySessionDelegate(), new NSOperationQueue());

        var downloadURL = NSUrl.FromString(url);
        var request = NSUrlRequest.FromUrl(downloadURL);
        var downloadTask = session.CreateDownloadTask(request);
    }
}

public class MySessionDelegate : NSObject, INSUrlSessionDownloadDelegate
{
    public void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
    {

    }

    public void DidWriteData(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite)
    {
        //Console.WriteLine(string.Format("DownloadTask: {0}  progress: {1}", downloadTask, progress));
        //InvokeOnMainThread(() => {
        //    // update UI with progress bar, if desired
        //});
    }
}
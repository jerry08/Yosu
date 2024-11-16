using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yosu.Services;

public class DownloadServiceImpl : IDownloadService
{
    public Task EnqueueAsync(
        string fileName,
        string url,
        Dictionary<string, string>? headers = null
    )
    {
        throw new NotImplementedException();
    }
}

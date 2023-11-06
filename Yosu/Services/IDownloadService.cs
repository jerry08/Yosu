using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yosu.Services;

public interface IDownloadService
{
    Task EnqueueAsync(string fileName, string url, Dictionary<string, string>? headers = null);
}

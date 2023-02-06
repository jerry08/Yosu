using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using SoundCloudExplode;
using SoundCloudExplode.Track;

namespace Yosu.Soundcloud.Core.Downloading;

public class TrackDownloader
{
    private readonly SoundCloudClient _soundcloud = new();

    public async Task DownloadAsync(
        string filePath,
        TrackInformation track,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var dirPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(dirPath))
            Directory.CreateDirectory(dirPath);

        await _soundcloud.DownloadAsync(
            track,
            filePath,
            progress?.ToDoubleBased(),
            cancellationToken
        );
    }
}
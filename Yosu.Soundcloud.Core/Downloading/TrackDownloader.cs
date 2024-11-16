using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using SoundCloudExplode;
using SoundCloudExplode.Tracks;

namespace Yosu.Soundcloud.Core.Downloading;

public class TrackDownloader
{
    private readonly SoundCloudClient _soundcloud = new();

    private bool IsInitializing { get; set; }

    public async Task DownloadAsync(
        string filePath,
        Track track,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default
    )
    {
        while (IsInitializing)
        {
            await Task.Delay(100, cancellationToken);
        }

        IsInitializing = true;

        if (!_soundcloud.IsInitialized)
            await _soundcloud.InitializeAsync(cancellationToken);

        IsInitializing = false;

        var dirPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(dirPath))
            Directory.CreateDirectory(dirPath);

        await _soundcloud.DownloadAsync(
            track,
            filePath,
            progress?.ToDoubleBased(),
            null,
            cancellationToken
        );
    }
}

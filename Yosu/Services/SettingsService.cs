using System.IO;
using Cogwheel;
using Microsoft.Maui.Storage;
using Yosu.Youtube.Core.Downloading;
using YoutubeExplode.Videos.Streams;

namespace Yosu.Services;

public class SettingsService : SettingsBase
{
    public bool IsAutoUpdateEnabled { get; set; } = true;

    public bool ShouldInjectTags { get; set; } = true;

    public bool ShouldSkipExistingFiles { get; set; }

    public string FileNameTemplate { get; set; } = "$title";

    // FFmpeg only handles up to 2 at a time probably
    public int YoutubeParallelLimit { get; set; } = 1;

    public int SoundcloudParallelLimit { get; set; } = 5;

    public int SpotifyParallelLimit { get; set; } = 5;

    public Container LastContainer { get; set; } = Container.Mp4;

    public VideoQualityPreference LastVideoQualityPreference { get; set; } = VideoQualityPreference.Highest;

    public SettingsService()
        : base(Path.Combine(FileSystem.AppDataDirectory, "Settings.json"))
    {
    }
}
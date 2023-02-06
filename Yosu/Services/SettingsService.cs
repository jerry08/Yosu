using YoutubeExplode.Videos.Streams;
using Yosu.Settings;
using Yosu.Youtube.Core.Downloading;

namespace Yosu.Services;

public class SettingsService : SettingsManager
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
}
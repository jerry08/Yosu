using System.ComponentModel;
using System.IO;
using Cogwheel;
using Microsoft.Maui.Storage;
using PropertyChanged;
using Yosu.Youtube.Core.Downloading;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace Yosu.Services;

[AddINotifyPropertyChangedInterface]
public partial class SettingsService : SettingsBase, INotifyPropertyChanged
{
    public bool AlwaysCheckForUpdates { get; set; } = true;

    public string? DownloadDir { get; set; }

    public bool ShouldInjectTags { get; set; } = true;

    public bool ShouldSkipExistingFiles { get; set; }

    public string SoundCloudFileNameTemplate { get; set; } = "$title";

    public string SpotifyFileNameTemplate { get; set; } = "$title";

    public string YoutubeFileNameTemplate { get; set; } = "$title";

    public Container LastContainer { get; set; } = Container.Mp4;

    public VideoQualityPreference LastVideoQualityPreference { get; set; } = VideoQualityPreference.Highest;

    // FFmpeg only handles up to 2 at a time probably
    public int ParallelLimit { get; set; } = 1;

    public SettingsService()
        : base(Path.Combine(FileSystem.AppDataDirectory, "Settings.json"))
    {
    }
}
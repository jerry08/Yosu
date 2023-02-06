using Yosu.Youtube.Core.Downloading;
using YoutubeExplode.Videos;

namespace Yosu.ViewModels.Components;

public class YoutubeDownloadViewModel : DownloadViewModelBase
{
    public string? Key => Video?.Id;

    public IVideo? Video { get; set; }

    public VideoDownloadOption? DownloadOption;

    public VideoDownloadPreference? DownloadPreference;
}
using Microsoft.Maui.Controls;
using SoundCloudExplode.Tracks;
using Yosu.ViewModels.Components;
using YoutubeExplode.Videos;

namespace Yosu.Views;

public class MainDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? YoutubeVideoDataTemplate { get; set; }

    public DataTemplate? SoundcloudDataTemplate { get; set; }

    public DataTemplate? SpotifyDataTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        //if (item is IVideo)
        //    return YoutubeVideoDataTemplate;
        //
        //if (item is Track)
        //    return SoundcloudDataTemplate;

        if (item is DownloadItem downloadItem)
        {
            if (downloadItem.Entity is YoutubeDownloadViewModel)
                return YoutubeVideoDataTemplate;

            if (downloadItem.Entity is SoundcloudDownloadViewModel)
                return SoundcloudDataTemplate;

            if (downloadItem.Entity is SpotifyDownloadViewModel)
                return SpotifyDataTemplate;
        }

        if (item is YoutubeDownloadViewModel)
            return YoutubeVideoDataTemplate;

        if (item is SoundcloudDownloadViewModel)
            return SoundcloudDataTemplate;

        if (item is SpotifyDownloadViewModel)
            return SpotifyDataTemplate;

        return null;
    }
}

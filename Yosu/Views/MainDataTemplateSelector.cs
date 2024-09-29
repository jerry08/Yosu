using Microsoft.Maui.Controls;
using Yosu.ViewModels.Components;

namespace Yosu.Views;

public class MainDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? YoutubeVideoDataTemplate { get; set; }

    public DataTemplate? SoundcloudDataTemplate { get; set; }

    public DataTemplate? SpotifyDataTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        if (item is DownloadItem downloadItem)
        {
            return downloadItem.SourceType switch
            {
                SourceType.Youtube => YoutubeVideoDataTemplate,
                SourceType.Soundcloud => SoundcloudDataTemplate,
                SourceType.Spotify => SpotifyDataTemplate,
                _ => null,
            };
        }

        return item switch
        {
            YoutubeDownloadViewModel => YoutubeVideoDataTemplate,
            SoundcloudDownloadViewModel => SoundcloudDataTemplate,
            SpotifyDownloadViewModel => SpotifyDataTemplate,
            _ => null,
        };
    }
}

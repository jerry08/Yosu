using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Yosu.Services;
using Yosu.Utils;
using Yosu.ViewModels.Framework;

namespace Yosu.ViewModels.Settings;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IStatusBarStyleManager _statusBarStyleManager;

    [ObservableProperty]
    private PreferenceService _preference = default!;

    [ObservableProperty]
    private SettingsService _settings = default!;

    public SettingsViewModel(
        PreferenceService preferenceService,
        SettingsService settingsService,
        IStatusBarStyleManager statusBarStyleManager)
    {
        _statusBarStyleManager = statusBarStyleManager;

        Preference = preferenceService;
        Preference.Load();

        Settings = settingsService;
        Settings.Load();

        Settings.PropertyChanged += (_, _) => Settings.Save();
    }

    [RelayCommand]
    private void ThemeSelected(int index)
    {
        Preference.AppTheme = (AppTheme)index;
        Preference.Save();
        App.ApplyTheme();

        _statusBarStyleManager.SetDefault();

        Application.Current.MainPage = new AppShell();
    }

    [RelayCommand]
    private async void SoundcloudFileNameTemplateTapped()
    {
        var result = await Shell.Current.DisplayPromptAsync(
            "Soundcloud file name template",
            """
            Template used for generating file names for downloaded tracks.

            Available tokens:
            $num - track's position in the list (if applicable)

            $id - track ID

            $title - track title

            $artist - track artist

            $album - track album/playlist name if available. It's only available when downloading tracks from an album/playlist url.
            """,
            initialValue: Settings.SoundCloudFileNameTemplate);

        if (string.IsNullOrWhiteSpace(result))
            return;

        Settings.SoundCloudFileNameTemplate = result;
    }

    [RelayCommand]
    private async void YoutubeFileNameTemplateTapped()
    {
        var result = await Shell.Current.DisplayPromptAsync(
            "Youtube file name template",
            """
            Template used for generating file names for downloaded videos and audio.

            Available tokens:
            $num — video's position in the list (if applicable)

            $id — video ID

            $title - video title

            $author - video author
            """,
            initialValue: Settings.YoutubeFileNameTemplate);

        if (string.IsNullOrWhiteSpace(result))
            return;

        Settings.YoutubeFileNameTemplate = result;
    }

    [RelayCommand]
    private async void SpotifyFileNameTemplateTapped()
    {
        var result = await Shell.Current.DisplayPromptAsync(
            "Spotify file name template",
            """
            Template used for generating file names for downloaded tracks.

            Available tokens:
            $num - track's position in the list (if applicable)

            $id - track ID

            $title - track title

            $artist - track artist

            $album - track album/playlist name if available. It's only available when downloading tracks from an album/playlist url.
            """,
            initialValue: Settings.SpotifyFileNameTemplate);

        if (string.IsNullOrWhiteSpace(result))
            return;

        Settings.SpotifyFileNameTemplate = result;
    }

    [RelayCommand]
    private async void Github()
    {
        await Browser.Default.OpenAsync("https://github.com/jerry08/Yosu");
    }

    [RelayCommand]
    private async void Discord()
    {
        await Browser.Default.OpenAsync("https://discord.gg/mhxsSMy2Nf");
    }
}
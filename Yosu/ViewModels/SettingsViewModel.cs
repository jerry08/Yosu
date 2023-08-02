using System.IO;
using System.Threading.Tasks;
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

    [ObservableProperty]
    private string _defaultDownloadDirectory = default!;

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

#if ANDROID
        DefaultDownloadDirectory = Path.Combine(
            Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath,
            "Yosu"
        );

        Settings.DownloadDir ??= DefaultDownloadDirectory;
#endif
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
    private async Task PickDownloadLocation()
    {
#if ANDROID
        var result = await Shell.Current.DisplayActionSheet(
            "Choose location",
            "Cancel",
            null,
            DefaultDownloadDirectory,
            "Pick location"
        );

        if (result == DefaultDownloadDirectory)
        {
            Settings.DownloadDir = DefaultDownloadDirectory;
            DefaultDownloadDirectory = Settings.DownloadDir;
            return;
        }

        if (result != "Pick location")
            return;

        if (Platform.CurrentActivity is MainActivity activity)
        {
            var res = await activity.PickDirectoryAsync();
            if (res.IsSuccess && !string.IsNullOrWhiteSpace(res.Data?.Data?.Path))
            {
                Settings.DownloadDir = res.Data.Data.Path;
                DefaultDownloadDirectory = Settings.DownloadDir;
            }
        }
#endif
    }

    [RelayCommand]
    private async Task SoundcloudFileNameTemplateTapped()
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
    private async Task YoutubeFileNameTemplateTapped()
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
    private async Task SpotifyFileNameTemplateTapped()
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
    private async Task Github()
    {
        await Browser.Default.OpenAsync("https://github.com/jerry08/Yosu");
    }

    [RelayCommand]
    private async Task Discord()
    {
        await Browser.Default.OpenAsync("https://discord.gg/mhxsSMy2Nf");
    }
}
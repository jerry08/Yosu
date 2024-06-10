using System;
using System.Linq;
using System.Threading.Tasks;
using Berry.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using SoundCloudExplode.Exceptions;
using SpotifyExplode.Exceptions;
using Woka.Utils.Extensions;
using Yosu.Services;
using Yosu.Utils;
using Yosu.ViewModels.Components;
using Yosu.ViewModels.Framework;
using Yosu.Views;
using Yosu.Views.BottomSheets;
using Yosu.Views.Settings;
using YoutubeExplode.Exceptions;

namespace Yosu.ViewModels;

// Use a list of objects to set ItemsSource in CollectionView. Setting to another type
// will trigger a bug where CollectionView will not update SelectedItems bound property. (Issue #8435)
public partial class MainCollectionViewModel : CollectionViewModel<object>
{
    private readonly PreferenceService _preference;

    private readonly QueryResolver _queryResolver = new();

    private readonly YoutubeViewModel _youtubeViewModel;
    private readonly SoundcloudViewModel _soundcloudViewModel;
    private readonly SpotifyViewModel _spotifyViewModel;

    [ObservableProperty]
    private SourceType _searchSourceType;

    private SourceType SourceType =>
        SelectedEntities.FirstOrDefault() switch
        {
            YoutubeDownloadViewModel => SourceType.Youtube,
            SoundcloudDownloadViewModel => SourceType.Soundcloud,
            SpotifyDownloadViewModel => SourceType.Spotify,
            _ => SourceType.None,
        };

    private SearchOptionsView? SearchOptionsView { get; set; }

    public static string? IntentUrl { get; set; }

    public MainCollectionViewModel(
        PreferenceService preference,
        YoutubeViewModel youtubeViewModel,
        SoundcloudViewModel soundcloudViewModel,
        SpotifyViewModel spotifyViewModel
    )
    {
        Title = "Youtube";
        IsBusy = false;

        _youtubeViewModel = youtubeViewModel;
        _soundcloudViewModel = soundcloudViewModel;
        _spotifyViewModel = spotifyViewModel;

        _preference = preference;
        _preference.Load();

        SearchSourceType = _preference.SearchSourceType;

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SearchSourceType))
                SearchSourceTypeChanged();
        };

        Query = IntentUrl;
    }

    protected override void LoadCore()
    {
        base.LoadCore();
        ProcessQuery();
    }

    [RelayCommand]
    async Task ProcessQuery()
    {
        if (!string.IsNullOrWhiteSpace(IntentUrl))
        {
            Query = IntentUrl;
            IntentUrl = null;
        }

        KeyboardManager.HideKeyboard();

        if (string.IsNullOrWhiteSpace(Query))
        {
            IsRefreshing = false;
            IsBusy = false;
            return;
        }

        if (!await IsOnline())
            return;

        if (!IsRefreshing)
            IsBusy = true;

        try
        {
            var result = await Task.Run(() => _queryResolver.ResolveAsync(Query, SearchSourceType));

            Entities.Clear();

            if (result.Count == 0)
            {
                await App.AlertSvc.ShowAlertAsync(
                    "Nothing found",
                    "Couldn't find any media based on the query or URL you provided"
                );
                return;
            }

            Push(result);

            RefreshDownloadingItems();
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync(
                "Error",
                // Short error message for YouTube-related errors, Soundcloud-related errors,
                // Spotify-related errors, full for others
                ex is YoutubeExplodeException
                || ex is SoundcloudExplodeException
                || ex is SpotifyExplodeException
                    ? ex.Message
                    : ex.ToString()
            );
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    async Task Download(DownloadViewModelBase entity)
    {
        if (!await IsOnline())
            return;

        var status = await StoragePermissionUtil.CheckAndRequestStoragePermission();
        if (status == PermissionStatus.Denied)
        {
            await Snackbar.Make("Storage permission not granted.").Show();
            return;
        }

        try
        {
            switch (entity)
            {
                case YoutubeDownloadViewModel download:
                    _youtubeViewModel.ShowOptions([download]);
                    break;

                case SoundcloudDownloadViewModel download:
                    _soundcloudViewModel.EnqueueDownloads(new[] { download });
                    break;

                case SpotifyDownloadViewModel download:
                    _spotifyViewModel.EnqueueDownloads(new[] { download });
                    break;
            }
        }
        catch
        {
            await Snackbar.Make("Download failed").Show();
        }
    }

    [RelayCommand]
    async Task DownloadSelected()
    {
        if (!await IsOnline())
            return;

        var status = await StoragePermissionUtil.CheckAndRequestStoragePermission();
        if (status == PermissionStatus.Denied)
        {
            await Snackbar.Make("Storage permission not granted.").Show();
            return;
        }

        try
        {
            switch (SourceType)
            {
                case SourceType.Youtube:
                    _youtubeViewModel.ShowOptions(
                        SelectedEntities.OfType<YoutubeDownloadViewModel>().ToList()
                    );
                    break;

                case SourceType.Soundcloud:
                    _soundcloudViewModel.EnqueueDownloads(
                        SelectedEntities.OfType<SoundcloudDownloadViewModel>()
                    );
                    //await Toast.Make("Download started").Show();
                    break;

                case SourceType.Spotify:
                    _spotifyViewModel.EnqueueDownloads(
                        SelectedEntities.OfType<SpotifyDownloadViewModel>()
                    );
                    break;
            }

            SelectionMode = SelectionMode.None;
            SelectedEntities.Clear();
        }
        catch { }
    }

    [RelayCommand]
    void CancelDownload(DownloadViewModelBase download)
    {
        download.Cancel();
    }

    [RelayCommand]
    async Task RestartDownload(DownloadViewModelBase entity)
    {
        if (!await IsOnline())
            return;

        switch (entity)
        {
            case YoutubeDownloadViewModel download:
                _youtubeViewModel.RestartDownload(download);
                break;

            case SoundcloudDownloadViewModel download:
                _soundcloudViewModel.RestartDownload(download);
                break;

            case SpotifyDownloadViewModel download:
                _spotifyViewModel.RestartDownload(download);
                break;
        }
    }

    [RelayCommand]
    async Task OpenUrl(DownloadViewModelBase entity)
    {
        var url = GetUrl(entity);
        if (url is null)
            return;

        await Browser.Default.OpenAsync(url);
    }

    [RelayCommand]
    async Task CopyUrl(DownloadViewModelBase entity)
    {
        var url = GetUrl(entity);
        if (url is null)
            return;

        await Clipboard.Default.SetTextAsync(url);
        await Snackbar.Make("Copied").Show();
    }

    private string? GetUrl(DownloadViewModelBase entity)
    {
        if (SelectionMode != SelectionMode.None)
            return null;

        return entity switch
        {
            YoutubeDownloadViewModel download => download.Video?.Url,
            SoundcloudDownloadViewModel download => download.Track?.PermalinkUrl?.ToString(),
            SpotifyDownloadViewModel download => download.Track?.Url,
            _ => null
        };
    }

    [RelayCommand]
    async Task ViewErrorMessage(DownloadViewModelBase download)
    {
        await App.AlertSvc.ShowAlertAsync("Error", $"{download.ErrorMessage}");
    }

    [RelayCommand]
    async Task ShowSearchOptions()
    {
        SearchOptionsView = new SearchOptionsView(this);
        await SearchOptionsView.ShowAsync();
    }

    private async void SearchSourceTypeChanged()
    {
        if (SearchOptionsView is not null)
            await SearchOptionsView.DismissAsync();

        _preference.Load();
        _preference.SearchSourceType = SearchSourceType;
        _preference.Save();

        Entities.Clear();
        Refresh();
    }

    [RelayCommand]
    async Task GoToHistory()
    {
        await Shell.Current.GoToAsyncSingle(nameof(HistoryCollectionView));
    }

    [RelayCommand]
    async Task GoToSettings()
    {
        await Shell.Current.GoToAsyncSingle($"Settings/{nameof(SettingsPage)}");
    }

    [RelayCommand]
    async Task Donate()
    {
        await Browser.Default.OpenAsync("https://www.buymeacoffee.com/jerry08");
    }
}

public partial class MainCollectionViewModel : CollectionViewModel<object>
{
    public void RefreshDownloadingItems()
    {
        for (var i = 0; i < Entities.Count; i++)
        {
            switch (Entities[i])
            {
                case YoutubeDownloadViewModel download:
                    //var existingYtDownload = _youtubeViewModel.Downloads
                    //    .FirstOrDefault(x => x.Video?.Id == download.Video?.Id);
                    var existingYtDownload = YoutubeViewModel.Downloads.FirstOrDefault(x =>
                        x.Key == download.Key
                    );
                    if (existingYtDownload is not null)
                    {
                        Entities[i] = existingYtDownload;
                    }
                    break;

                case SoundcloudDownloadViewModel download:
                    var existingScDownload = SoundcloudViewModel.Downloads.FirstOrDefault(x =>
                        x.Key == download.Key
                    );
                    if (existingScDownload is not null)
                    {
                        Entities[i] = existingScDownload;
                    }
                    break;

                case SpotifyDownloadViewModel download:
                    var existingSpDownload = SpotifyViewModel.Downloads.FirstOrDefault(x =>
                        x.Key == download.Key
                    );
                    if (existingSpDownload is not null)
                    {
                        Entities[i] = existingSpDownload;
                    }
                    break;
            }
        }
    }
}

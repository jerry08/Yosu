using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Yosu.Extensions;
using Yosu.Services;
using Yosu.ViewModels.Components;
using Yosu.ViewModels.Framework;
using Yosu.Views;
using YoutubeExplode.Videos;
using SoundcloudTrack = SoundCloudExplode.Tracks.Track;
using SpotifyTrack = SpotifyExplode.Tracks.Track;

namespace Yosu.ViewModels;

public partial class HistoryCollectionViewModel : CollectionViewModel<ListGroup<object>>
{
    private readonly PreferenceService _preference;

    public HistoryCollectionViewModel(PreferenceService preference)
    {
        _preference = preference;

        Title = "Download History";
        IsBusy = true;
    }

    protected override void LoadCore()
    {
        base.LoadCore();
        ProcessQuery();
    }

    [RelayCommand]
    async Task ProcessQuery()
    {
        //KeyboardHelper.HideKeyboard();

        //if (string.IsNullOrWhiteSpace(Query))
        //{
        //    IsRefreshing = false;
        //    IsBusy = false;
        //    return;
        //}

        if (!IsRefreshing)
            IsBusy = true;

        try
        {
            _preference.Load();

            Entities.Clear();

            //var test1 = JsonConvert.SerializeObject(_preference.Downloads);
            //var test2 = JsonConvert.DeserializeObject<List<DownloadItem>>(test1);

            var newList = _preference
                .Downloads.Where(x => x.Entity is not null)
                .Select(x =>
                {
                    var entityStr = JsonSerializer.Serialize(x.Entity!);

                    switch (x.SourceType)
                    {
                        case SourceType.Youtube:
                            x.Entity = new YoutubeDownloadViewModel()
                            {
                                Video = JsonSerializer.Deserialize<Video>(entityStr)
                            };
                            break;

                        case SourceType.Soundcloud:
                            x.Entity = new SoundcloudDownloadViewModel()
                            {
                                Track = JsonSerializer.Deserialize<SoundcloudTrack>(entityStr)
                            };
                            break;

                        case SourceType.Spotify:
                            x.Entity = new SpotifyDownloadViewModel()
                            {
                                Track = JsonSerializer.Deserialize<SpotifyTrack>(entityStr)
                            };
                            break;
                    }

                    //x.Entity = x.SourceType switch
                    //{
                    //    SourceType.Youtube => JsonConvert.DeserializeObject<Video>(entityStr),
                    //    SourceType.Soundcloud => JsonConvert.DeserializeObject<Track>(entityStr),
                    //    SourceType.Spotify => JsonConvert.DeserializeObject<Track>(entityStr),
                    //    _ => null,
                    //};

                    return x;
                    //return null;
                })
                .Where(x =>
                    Query is null
                    || (
                        x.Entity is YoutubeDownloadViewModel ytDownload
                        && ytDownload.Video?.Title?.Contains(
                            Query,
                            StringComparison.OrdinalIgnoreCase
                        ) == true
                    )
                    || (
                        x.Entity is SoundcloudDownloadViewModel scDownload
                        && (
                            scDownload.Track?.Title?.Contains(
                                Query,
                                StringComparison.OrdinalIgnoreCase
                            ) == true
                            || scDownload.Track?.User?.Username?.Contains(
                                Query,
                                StringComparison.OrdinalIgnoreCase
                            ) == true
                        )
                    )
                    || (
                        x.Entity is SpotifyDownloadViewModel spDownload
                        && (
                            spDownload.Track?.Title?.Contains(
                                Query,
                                StringComparison.OrdinalIgnoreCase
                            ) == true
                            || spDownload
                                .Track?.Artists.FirstOrDefault()
                                ?.Name.Contains(Query, StringComparison.OrdinalIgnoreCase) == true
                        )
                    )
                )
                .Select(x => x!)
                .ToList();

            if (newList.Count > 0)
            {
                var list = newList
                    .OrderByDescending(x => x.DownloadDate)
                    //.GroupBy(x => x.DownloadDate.StartOfWeek(DayOfWeek.Sunday))
                    //.GroupBy(x => $"{x.DownloadDate.StartOfWeek(DayOfWeek.Sunday)}-{x.SourceType}")
                    .GroupBy(x =>
                        $"{x.DownloadDate.DayOfYear + x.DownloadDate.Year}-{x.SourceType}"
                    )
                    .Select(x =>
                    {
                        var firstItem = x.First()!;
                        var name =
                            $"{firstItem.SourceType} - {firstItem.DownloadDate:MMMM dd, yyyy}";

                        return new ListGroup<object>(name, x.Select(x => x.Entity!).ToList());
                    })
                    .ToList();

                Push(list);

                //RefreshDownloadingItems();
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.ToString());
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    async Task ClearAll()
    {
        var result = await App.AlertSvc.ShowConfirmationAsync(
            "Confirmation",
            "Are you sure you want to clear all?"
        );

        if (!result)
            return;

        _preference.Downloads.Clear();
        _preference.Save();

        Load();
    }

    [RelayCommand]
    async Task ClearSelected()
    {
        if (SelectedEntities.Count == 0)
        {
            await App.AlertSvc.ShowAlertAsync("", "Nothing selected");
            return;
        }

        var result = await App.AlertSvc.ShowConfirmationAsync(
            "Confirmation",
            "Are you sure you want to clear all selected items?"
        );

        if (!result)
            return;

        _preference.Downloads.Clear();
        _preference.Save();

        Load();
    }

    [RelayCommand]
    async Task Export()
    {
        if (Platform.CurrentActivity is MainActivity activity)
        {
            var result = await activity.PickDirectoryAsync();
            if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.Data?.Data?.Path))
            {
                var path = result.Data.Data.Path;
                var newFilePath = Path.Combine(path, "yosu-history.json");

                var tempFilePath = Path.Combine(
                    FileSystem.CacheDirectory,
                    $"{DateTime.Now.Ticks}.json"
                );

                var popup = new LoadingPopup(
                    new LoadingPopupViewModel { LoadingText = "Exporting..." }
                );
                Application.Current?.MainPage?.ShowPopup(popup);

                try
                {
                    _preference.Load();

                    var jsonData = JsonSerializer.Serialize(_preference.Downloads);

                    using (var writer = File.CreateText(tempFilePath))
                    {
                        await writer.WriteLineAsync(jsonData);
                    }

                    var tryCount = 1;
                    while (activity.FileExists(newFilePath))
                    {
                        tryCount++;
                        newFilePath = Path.Combine(path, $"yosu-history ({tryCount}).json");
                    }

                    await activity.CopyFileAsync(tempFilePath, newFilePath);

                    await Toast.Make("Export completed").Show();

                    File.Delete(tempFilePath);
                }
                catch
                {
                    await Toast.Make("Export failed").Show();

                    try
                    {
                        // Delete file
                        if (!string.IsNullOrEmpty(tempFilePath))
                            File.Delete(tempFilePath);
                    }
                    catch
                    {
                        // Ignore
                    }
                }
                finally
                {
                    popup.Close();
                }
            }
        }
    }

    [RelayCommand]
    async Task Import()
    {
        var customFileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/json" } },
            }
        );

        var options = new PickOptions
        {
            PickerTitle = "Please select a json file",
            FileTypes = customFileType,
        };

        var result = await FilePicker.PickAsync(options);
        if (result is null)
            return;

        try
        {
            _preference.Load();

            var stream = await result.OpenReadAsync();
            //var json = await stream.ToStringAsync();

            var list = await JsonSerializer.DeserializeAsync<List<DownloadItem>>(stream);
            if (list is not null)
            {
                _preference.Downloads.AddRange(list);
                _preference.Downloads = _preference
                    .Downloads.GroupBy(x => x.Key)
                    .Select(x => x.Last())
                    .ToList();
                _preference.Save();

                await Toast.Make("History imported successfully").Show();

                Refresh();
            }
        }
        catch (Exception ex)
        {
            await Toast.Make("Import failed").Show();
        }
    }
}

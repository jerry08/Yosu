using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoundCloudExplode.Track;
using SpotifyExplode.Tracks;
using Yosu.Platforms.Services;
using Yosu.Services;
using Yosu.Utils.Extensions;
using Yosu.ViewModels.Components;
using Yosu.ViewModels.Framework;
using YoutubeExplode.Videos;

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
    async void ProcessQuery()
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
            await _preference.LoadAsync();

            Entities.Clear();

            //var test1 = JsonConvert.SerializeObject(_preference.Downloads);
            //var test2 = JsonConvert.DeserializeObject<List<DownloadItem>>(test1);

            var newList = _preference.Downloads
                .Where(x => x.Entity is not null)
                .Select(x =>
                {
                    var jObj = JObject.FromObject(x.Entity!);

                    switch (x.SourceType)
                    {
                        case SourceType.Youtube:
                            x.Entity = new YoutubeDownloadViewModel()
                            {
                                Video = JsonConvert.DeserializeObject<Video>(jObj.ToString())
                            };
                            break;

                        case SourceType.Soundcloud:
                            x.Entity = new SoundcloudDownloadViewModel()
                            {
                                Track = JsonConvert.DeserializeObject<TrackInformation>(jObj.ToString())
                            };
                            break;

                        case SourceType.Spotify:
                            x.Entity = new SpotifyDownloadViewModel()
                            {
                                Track = JsonConvert.DeserializeObject<Track>(jObj.ToString())
                            };
                            break;
                    }

                    //x.Entity = x.SourceType switch
                    //{
                    //    SourceType.Youtube => JsonConvert.DeserializeObject<Video>(jObj.ToString()),
                    //    SourceType.Soundcloud => JsonConvert.DeserializeObject<TrackInformation>(jObj.ToString()),
                    //    SourceType.Spotify => JsonConvert.DeserializeObject<Track>(jObj.ToString()),
                    //    _ => null,
                    //};

                    return x;
                    //return null;
                })
                .Where(x => Query is null
                    || (x.Entity is YoutubeDownloadViewModel ytDownload
                        && ytDownload.Video?.Title?.Contains(Query, StringComparison.OrdinalIgnoreCase) == true)
                    || (x.Entity is SoundcloudDownloadViewModel scDownload
                        && (scDownload.Track?.Title?.Contains(Query, StringComparison.OrdinalIgnoreCase) == true
                            || scDownload.Track?.User?.Username?.Contains(Query, StringComparison.OrdinalIgnoreCase) == true))
                    || (x.Entity is SpotifyDownloadViewModel spDownload
                        && (spDownload.Track?.Title?.Contains(Query, StringComparison.OrdinalIgnoreCase) == true
                            || spDownload.Track?.Artists.FirstOrDefault()?.Name.Contains(Query, StringComparison.OrdinalIgnoreCase) == true)))
                .Select(x => x!).ToList();

            if (newList.Count > 0)
            {
                var list = newList.OrderByDescending(x => x.DownloadDate)
                    //.GroupBy(x => x.DownloadDate.StartOfWeek(DayOfWeek.Sunday))
                    //.GroupBy(x => $"{x.DownloadDate.StartOfWeek(DayOfWeek.Sunday)}-{x.SourceType}")
                    .GroupBy(x => $"{x.DownloadDate.DayOfYear + x.DownloadDate.Year}-{x.SourceType}")
                    .Select(x =>
                    {
                        var firstItem = x.First()!;
                        var name = $"{firstItem.SourceType} - {firstItem.DownloadDate:MMMM dd, yyyy}";

                        return new ListGroup<object>(
                            name,
                            x.Select(x => x.Entity!).ToList()
                        );
                    }).ToList();

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
    async void ClearAll()
    {
        var result = await App.AlertSvc.ShowConfirmationAsync(
            "Confirmation",
            "Are you sure you want to clear all?"
        );

        if (!result)
            return;

        _preference.Downloads.Clear();
        await _preference.SaveAsync();

        Load();
    }

    [RelayCommand]
    async void ClearSelected()
    {
        _preference.Downloads.Clear();
        await _preference.SaveAsync();

        Load();
    }
}
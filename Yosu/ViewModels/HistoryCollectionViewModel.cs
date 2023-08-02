using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using SoundCloudExplode.Tracks;
using SpotifyExplode.Tracks;
using Yosu.Services;
using Yosu.ViewModels.Components;
using Yosu.ViewModels.Framework;
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

            var newList = _preference.Downloads
                .Where(x => x.Entity is not null)
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
    async Task ClearAll()
    {
        var result = await App.AlertSvc.ShowConfirmationAsync(
            "Confirmation",
            "Are you sure you want to clear all?"
        );

        if (!result)
            return;

        _preference.Downloads.Clear();
        _preference.Load();

        Load();
    }

    [RelayCommand]
    void ClearSelected()
    {
        _preference.Downloads.Clear();
        _preference.Save();

        Load();
    }
}
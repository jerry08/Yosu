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
using Yosu.Data;
using Yosu.Extensions;
using Yosu.Services;
using Yosu.ViewModels.Components;
using Yosu.ViewModels.Framework;
using Yosu.Views;
using static Android.Provider.MediaStore;

namespace Yosu.ViewModels;

public partial class HistoryCollectionViewModel : CollectionViewModel<ListGroup<object>>
{
    private readonly HistoryDatabase _historyDatabase;

    public HistoryCollectionViewModel(HistoryDatabase historyDatabase)
    {
        _historyDatabase = historyDatabase;

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
            Entities.Clear();

            var downloads = await _historyDatabase.GetItemsAsync();

            downloads = downloads
                .Where(x =>
                    Query is null
                    || x.Title?.Contains(Query, StringComparison.OrdinalIgnoreCase) == true
                    || x.Author?.Contains(Query, StringComparison.OrdinalIgnoreCase) == true
                )
                .ToList();

            if (downloads.Count > 0)
            {
                downloads.ForEach(download => download.SetEntity());

                var list = downloads
                    .OrderByDescending(x => x.DownloadDate)
                    .GroupBy(x =>
                        $"{x.DownloadDate.DayOfYear + x.DownloadDate.Year}-{x.SourceType}"
                    )
                    .Select(x =>
                    {
                        var item = x.First();
                        var name = $"{item.SourceType} - {item.DownloadDate:MMMM dd, yyyy}";

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

        await _historyDatabase.DeleteAllAsync();

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

        //await _historyDatabase.DeleteAllAsync();

        Load();
    }

    [RelayCommand]
    async Task Export()
    {
#if ANDROID
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
                    var downloads = await _historyDatabase.GetItemsAsync();

                    var jsonData = JsonSerializer.Serialize(downloads);

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
#endif
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
            var stream = await result.OpenReadAsync();
            //var json = await stream.ToStringAsync();

            var list = await JsonSerializer.DeserializeAsync<List<DownloadItem>>(stream);

            ArgumentNullException.ThrowIfNull(list);

            var downloads = await _historyDatabase.GetItemsAsync();
            var existingIds = list.Select(x => x.Id).ToArray();

            list = list.Where(x => !existingIds.Contains(x.Id)).ToList();

            await _historyDatabase.AddItemsAsync(list);

            await Toast.Make("History imported successfully").Show();

            Refresh();
        }
        catch
        {
            await Toast.Make("Import failed").Show();
        }
    }
}

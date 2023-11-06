using System.Threading.Tasks;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.ContextMenuContainer;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Controls;

namespace Yosu.ViewModels.Framework;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private string? _subtitle;

    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty]
    private string? _profilePicPath;

    [RelayCommand]
    public async Task Load()
    {
        IsInitialized = true;

        //Apply delay for smooth loading, e.g. when logging out and logging back in
        //navigating to the dashboard will be smooth when loading data
        await Task.Delay(1);

        LoadCore();
    }

    protected virtual void LoadCore() { }

    public virtual void OnNavigated() { }

    [RelayCommand]
    void ShowContextMenu(ContextMenuContainer menu)
    {
        menu.Show();
    }

    public async Task<bool> IsOnline(bool showAlert = true)
    {
        var isOnline = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        if (!isOnline)
        {
            IsBusy = false;
            IsRefreshing = false;

            if (showAlert)
                await ShowNoInternetAlert();
        }

        return isOnline;
    }

    private Task ShowNoInternetAlert()
    {
        return App.AlertSvc.ShowAlertAsync(
            "No Internet Connection.",
            "If the problem persists, please try again later.",
            "CLOSE"
        );
    }

    [RelayCommand]
    public async Task GoBack(Dictionary<string, object>? parameters = null)
    {
        if (parameters is null)
            await Shell.Current.Navigation.PopAsync();
        else
            await Shell.Current.GoToAsync("..", parameters);
    }
}

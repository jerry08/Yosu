using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Yosu.Utils;
using Yosu.Services;
using Microsoft.Maui.Controls;
using Yosu.ViewModels.Framework;

namespace Yosu.ViewModels.Settings;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IStatusBarStyleManager _statusBarStyleManager;

    [ObservableProperty]
    private PreferenceService _preference = default!;

    public SettingsViewModel(
        PreferenceService preferenceService,
        IStatusBarStyleManager statusBarStyleManager)
    {
        _statusBarStyleManager = statusBarStyleManager;

        Preference = preferenceService;
        Preference.Load();
    }

    [RelayCommand]
    async void ThemeSelected(int index)
    {
        Preference.AppTheme = (AppTheme)index;
        await Preference.SaveAsync();
        App.ApplyTheme();

        _statusBarStyleManager.SetDefault();

        Application.Current.MainPage = new AppShell();
    }

    [RelayCommand]
    async void Github()
    {
        await Browser.Default.OpenAsync("https://github.com/jerry08/Yosu");
    }

    [RelayCommand]
    async void Discord()
    {
        await Browser.Default.OpenAsync("https://discord.gg/mhxsSMy2Nf");
    }
}
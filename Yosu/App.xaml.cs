using System;
using System.Linq;
using Berry.Maui.Behaviors;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Networking;
using Yosu.Services;
using Yosu.Services.AlertDialog;
using NavigationBarStyle = Berry.Maui.Behaviors.NavigationBarStyle;

namespace Yosu;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static IAlertService AlertSvc { get; private set; } = default!;

    public static bool IsChangingTheme { get; set; }

    public App(IServiceProvider provider)
    {
        InitializeComponent();

        Services = provider;
        AlertSvc = Services.GetRequiredService<IAlertService>();

        ApplyTheme();
    }

    private Window? CurrentWindow { get; set; }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (CurrentWindow is not null && IsChangingTheme)
        {
            IsChangingTheme = false;
            return CurrentWindow;
        }

        CurrentWindow = new(new AppShell());

        return CurrentWindow;
    }

    public static bool IsOnline(bool showSnackbar = true)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            if (showSnackbar)
            {
                Snackbar
                    .Make(
                        "No interent",
                        visualOptions: new SnackbarOptions()
                        {
                            BackgroundColor = Colors.Red,
                            TextColor = Colors.White,
                            ActionButtonTextColor = Colors.White,
                        }
                    )
                    .Show();
            }

            return false;
        }
        else
        {
            if (showSnackbar)
            {
                Snackbar
                    .Make(
                        "You're back online",
                        visualOptions: new SnackbarOptions()
                        {
                            BackgroundColor = Colors.Green,
                            TextColor = Colors.White,
                            ActionButtonTextColor = Colors.White,
                        }
                    )
                    .Show();
            }

            return true;
        }
    }

    public static async void CheckForUpdate()
    {
        var settingsService = new SettingsService();
        settingsService.Load();

        if (settingsService.AlwaysCheckForUpdates)
        {
            var updater = new AppUpdater();
            await updater.CheckAsync();
        }
    }

    public static void ApplyTheme()
    {
        var preferenceService = new PreferenceService();
        preferenceService.Load();

        if (Current is not null)
            Current.UserAppTheme = preferenceService.AppTheme;
    }

    public static void RefreshCurrentPageBehaviors()
    {
        if (Current is null)
            return;

        var gray900Color = Current.Resources["Gray900"];

#if !MACCATALYST
#pragma warning disable CA1416
        foreach (var behavior in Shell.Current.Behaviors.OfType<StatusBarBehavior>())
        {
            behavior.SetAppTheme(
                StatusBarBehavior.StatusBarColorProperty,
                Colors.White,
                Colors.Black
            );
            behavior.SetAppTheme(
                StatusBarBehavior.StatusBarStyleProperty,
                StatusBarStyle.DarkContent,
                StatusBarStyle.LightContent
            );
        }
#pragma warning restore CA1416
#endif

        foreach (var behavior in Shell.Current.Behaviors.OfType<NavigationBarBehavior>())
        {
            behavior.SetAppTheme(
                NavigationBarBehavior.NavigationBarColorProperty,
                Colors.White,
                gray900Color
            );
            behavior.SetAppTheme(
                NavigationBarBehavior.NavigationBarStyleProperty,
                NavigationBarStyle.DarkContent,
                NavigationBarStyle.LightContent
            );
        }
    }

#if ANDROID
    public static void StartForeground()
    {
        var activity = Platform.CurrentActivity;
        if (activity is null)
            return;

        var intent = new Android.Content.Intent(activity, typeof(ForegroundService));

        //intent.PutExtra("fileName", fileName);

        //if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.O)
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            activity.StartForegroundService(intent);
        }
        else
        {
            activity.StartService(intent);
        }
    }

    public static void StopForeground()
    {
        var activity = Platform.CurrentActivity;
        if (activity is null)
            return;

        var intent = new Android.Content.Intent(activity, typeof(ForegroundService));

        intent.SetAction("kill");

        //intent.PutExtra("fileName", fileName);

        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            activity.StartForegroundService(intent);
        }
        else
        {
            activity.StartService(intent);
        }

        //if (!ApplicationEx.IsRunning())
        //    Current?.Quit();
    }
#endif
}

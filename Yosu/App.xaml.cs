using System;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Networking;
using Yosu.Services;
using Yosu.Services.AlertDialog;
using Yosu.Utils;

namespace Yosu;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static IAlertService AlertSvc { get; private set; } = default!;

    public App(IServiceProvider provider)
    {
        InitializeComponent();

        MainPage = new AppShell();

        Services = provider;
        AlertSvc = Services.GetService<IAlertService>()!;

        ApplyTheme();

        if (IsOnline())
            CheckForUpdate();

        Connectivity.Current.ConnectivityChanged += (s, e) =>
        {
            if (IsOnline())
                CheckForUpdate();
        };
    }

    public static bool IsOnline(bool showSnackbar = true)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            if (showSnackbar)
            {
                Snackbar.Make("No interent", visualOptions: new SnackbarOptions()
                {
                    BackgroundColor = Colors.Red,
                    TextColor = Colors.White,
                    ActionButtonTextColor = Colors.White
                }).Show();
            }

            return false;
        }
        else
        {
            if (showSnackbar)
            {
                Snackbar.Make("You're back online", visualOptions: new SnackbarOptions()
                {
                    BackgroundColor = Colors.Green,
                    TextColor = Colors.White,
                    ActionButtonTextColor = Colors.White
                }).Show();
            }

            return true;
        }
    }

    public static async void CheckForUpdate()
    {
        var updater = new AppUpdater();
        await updater.CheckAsync();
    }

    public static void ApplyTheme()
    {
        var preferenceService = new PreferenceService();
        preferenceService.Load();

        if (Current is not null)
            Current.UserAppTheme = preferenceService.AppTheme;
    }

#if ANDROID
    public static void StartForeground()
    {
        var activity = Platform.CurrentActivity;
        if (activity is null)
            return;

        var intent = new Android.Content.Intent(
            activity,
            typeof(Yosu.Platforms.Services.ForegroundService)
        );

        //intent.PutExtra("fileName", fileName);
        activity.StartForegroundService(intent);
    }

    public static void StopForeground()
    {
        var activity = Platform.CurrentActivity;
        if (activity is null)
            return;

        var intent = new Android.Content.Intent(
            activity,
            typeof(Yosu.Platforms.Services.ForegroundService)
        );

        intent.SetAction("kill");

        //intent.PutExtra("fileName", fileName);
        activity.StartForegroundService(intent);

        //if (!ApplicationEx.IsRunning())
        //    Current?.Quit();
    }
#endif
}
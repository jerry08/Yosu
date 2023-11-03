using Berry.Maui;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Plugin.ContextMenuContainer;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Woka;
using Yosu.Handlers;
using Yosu.Services;
using Yosu.Services.AlertDialog;
using Yosu.Utils;
using Yosu.ViewModels;
using Yosu.ViewModels.Settings;
using Yosu.Views;
using Yosu.Views.Settings;

namespace Yosu;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                fonts.AddFont("Sora-Bold.ttf", "SoraBold");
                fonts.AddFont("Sora-Medium.ttf", "SoraMedium");
                fonts.AddFont("Sora-Regular.ttf", "SoraRegular");
                fonts.AddFont("Sora-SemiBold.ttf", "SoraSemiBold");
                fonts.AddFont("Sora-Thin.ttf", "SoraThin");

                fonts.AddFont("MaterialIconsOutlined-Regular.otf", "Material");
                fonts.AddFont("fa-solid-900.ttf", "FaSolid");
            })
            .UseSkiaSharp()
            .UseBerry()
            .UseMauiCompatibility()
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<CheckBox, MaterialCheckBoxHandler>();
                handlers.AddHandler<Switch, MaterialSwitchHandler>();
#endif

                handlers.AddHandler(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer));
            })
            .ConfigureWorkarounds()
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android
                    .OnCreate((activity, bundle) =>
                    {
                        var manager = new StatusBarStyleManager();
                        manager.SetDefault();
                    }));
#elif IOS
                events.AddiOS(ios => ios
                    .OnActivated((app) =>
                    {
                        var manager = new StatusBarStyleManager();
                        manager.SetDefault();
                    }));
#endif
            });
        //.ConfigureKeyboardM();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<HistoryCollectionView>();

        // Viewmodels
        builder.Services.AddTransient<MainCollectionViewModel>();
        builder.Services.AddTransient<YoutubeViewModel>();
        builder.Services.AddTransient<SoundcloudViewModel>();
        builder.Services.AddTransient<SpotifyViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<HistoryCollectionViewModel>();

        // Services
        builder.Services.AddSingleton<IAlertService, AlertService>();
        //builder.Services.AddSingleton<PreferenceService>();
        builder.Services.AddScoped<PreferenceService>();
        builder.Services.AddSingleton<SettingsService>();

        builder.Services.AddSingleton<IStatusBarStyleManager, StatusBarStyleManager>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
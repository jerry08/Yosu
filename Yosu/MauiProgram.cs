using Berry.Maui;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Plugin.ContextMenuContainer;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Woka;
using Yosu.Data;
using Yosu.Services;
using Yosu.Services.AlertDialog;
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
            .ConfigureContextMenuContainer()
            .ConfigureWorkarounds()
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android =>
                    android.OnCreate(
                        (activity, bundle) =>
                        {
                            var manager = new StatusBarStyleManager();
                            manager.SetDefault();
                        }
                    )
                );
#elif IOS
                events.AddiOS(ios =>
                    ios.OnActivated(
                        (app) =>
                        {
                            var manager = new StatusBarStyleManager();
                            manager.SetDefault();
                        }
                    )
                );
#endif
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        RegisterViewsAndViewModels(builder.Services);
        RegisterYosuServices(builder.Services);

        return builder.Build();
    }

    static void RegisterViewsAndViewModels(in IServiceCollection services)
    {
        // Add Views
        services.AddTransient<MainPage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<HistoryCollectionView>();

        // View Models
        services.AddTransient<MainCollectionViewModel>();
        services.AddTransient<YoutubeViewModel>();
        services.AddTransient<SoundcloudViewModel>();
        services.AddTransient<SpotifyViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<HistoryCollectionViewModel>();
    }

    static void RegisterYosuServices(in IServiceCollection services)
    {
        // Services
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<IStatusBarStyleManager, StatusBarStyleManager>();
        //services.AddSingleton<PreferenceService>();
        services.AddScoped<PreferenceService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<HistoryDatabase>();
    }
}

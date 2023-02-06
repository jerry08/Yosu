using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Plugin.ContextMenuContainer;
using Yosu.Services;
using Yosu.Services.AlertDialog;
using Yosu.Utils;
using Yosu.ViewModels.Settings;
using Yosu.Views.Settings;
using Yosu.ViewModels;
using Yosu.Views;
using System.Collections.Generic;
using Woka;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Plugin.MauiTouchEffect.Effects;
using System.Linq;
using Android.Media.Effect;
using Yosu.Handlers;

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
            //.UseMaterialComponents(new List<string>
            //{
            //    //generally, we needs add 6 types of font families
            //    "Roboto-Regular.ttf",
            //    "Roboto-Italic.ttf",
            //    "Roboto-Medium.ttf",
            //    "Roboto-MediumItalic.ttf",
            //    "Roboto-Bold.ttf",
            //    "Roboto-BoldItalic.ttf",
            //})
            .UseSkiaSharp()
            .UseMauiCompatibility()
            .ConfigureMauiHandlers(handlers =>
            {
                // Register ALL handlers in the Xamarin Community Toolkit assembly
                handlers.AddCompatibilityRenderers(typeof(Plugin.MauiTouchEffect.Effects.TouchEffect).Assembly);

                handlers.AddHandler<CheckBox, MaterialCheckBoxHandler>();
                handlers.AddHandler<Switch, MaterialSwitchHandler>();

                handlers.AddHandler(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer));
                //handlers.AddCompatibilityRenderers(typeof(PullToRefreshLayout).Assembly);
                //handlers.AddCompatibilityRenderer(typeof(PullToRefreshLayout), typeof(PullToRefreshLayoutRenderer));

#if ANDROID
                //Workarounds
                //handlers.AddHandler<Entry, Handlers.EntryHandler>();
                //handlers.AddHandler<RefreshView, Handlers.CustomRefreshViewHandler>();
#endif
            })
            .ConfigureEffects(effects =>
            {
                effects.AddCompatibilityEffects(typeof(Plugin.MauiTouchEffect.Effects.TouchEffect).Assembly);
                effects.Add(typeof(Plugin.MauiTouchEffect.Effects.TouchEffect), typeof(Plugin.MauiTouchEffect.Effects.PlatformTouchEffect));
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

        //Microsoft.Maui.Handlers.RefreshViewHandler.Mapper.AppendToMapping("Test", (handler, v) =>
        //{
        //
        //});

        // Workaround for TouchEffect being lost when navigating multiple times or
        // when closing the app (with OnBackButtonPressed()) and reopening the cached view.
        Microsoft.Maui.Handlers.ElementHandler.ElementMapper.AppendToMapping("YosuElement", (handler, e) =>
        {
            if (e is Element element)
            {
                var touchEffects = element.Effects.OfType<TouchEffect>().ToList();
                for (int i = 0; i < touchEffects.Count; i++)
                {
                    element.Effects.Remove(touchEffects[i]);
                    element.Effects.Add(touchEffects[i]);
                }
            }
        });

        //Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<HistoryCollectionView>();

        //Viewmodels
        builder.Services.AddTransient<MainCollectionViewModel>();
        builder.Services.AddTransient<YoutubeViewModel>();
        builder.Services.AddTransient<SoundcloudViewModel>();
        builder.Services.AddTransient<SpotifyViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<HistoryCollectionViewModel>();

        //Services
        builder.Services.AddSingleton<IAlertService, AlertService>();
        //builder.Services.AddSingleton<PreferenceService>();
        builder.Services.AddScoped<PreferenceService>();
        builder.Services.AddSingleton<SettingsService>();

        builder.Services.AddSingleton<IStatusBarStyleManager, StatusBarStyleManager>();

        //Laerdal.FFmpeg.Android.Config.IgnoreSignal(Laerdal.FFmpeg.Android.Signal.Sigxcpu);
        //
        //var f1 = "/data/user/0/com.berry.yosu/cache/638102374309888262.tmp.stream-0.tmp";
        //var gg = System.IO.File.Exists(f1);
        //System.IO.File.Create(f1);
        //var gg2 = System.IO.File.Exists(f1);
        //
        ////var arguments = "-i /data/user/0/com.berry.yosu/cache/638102320032679702.tmp.stream-0.tmp -i /data/user/0/com.berry.yosu/cache/638102320032679702.tmp.stream-1.tmp -f mp4 -map 0 -map 1 -c:a copy -c:v copy -nostdin -y /data/user/0/com.berry.yosu/cache/638102320032679702.tmp";
        //var arguments = "-i /data/user/0/com.berry.yosu/cache/638102374309888262.tmp.stream-0.tmp -f mp3 -map 0 -b:a 165k -threads 8 -nostdin -y /data/user/0/com.berry.yosu/cache/638102370181029302.tmp";
        //var exitCode = Laerdal.FFmpeg.Android.FFmpeg.Execute(arguments);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
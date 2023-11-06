using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;

//using Plugin.KeyboardM;

namespace Yosu;

public static class HostExtensions
{
    public static MauiAppBuilder ConfigureKeyboardM(this MauiAppBuilder builder)
    {
#if ANDROID

        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<Entry, Handlers.MaterialEntryHandler>();
        });

        //PageHandler.PlatformViewFactory = (h) => new NotifyingContentViewGroup(h.Context);
#endif
        return builder;
    }
}

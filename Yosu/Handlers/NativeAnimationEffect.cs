using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#elif __ANDROID__
using View = Android.Views.View;
using Android.Util;
#elif __IOS__
using UIKit;
using CoreGraphics;
using Foundation;
#endif

namespace Yosu.Handlers;

public class NativeAnimationEffect : RoutingEffect
{
}

public class NativeAnimationPlatformEffect : PlatformEffect
{
    public NativeAnimationPlatformEffect() : base()
    {
    }

#if WINDOWS

    FrameworkElement? view;

    protected override void OnAttached()
    {
        // Get the Windows View corresponding to the Element that the effect is attached to
        view = Control ?? Container;
    }

    protected override void OnDetached()
    {
        //throw new NotImplementedException();
    }
#elif __ANDROID__

    View? view;

    protected override void OnAttached()
    {
        // Get the Android View corresponding to the Element that the effect is attached to
        view = Control ?? Container;

        if (view is null)
            return;

        //Custom ripple effect
        view.Clickable = true;

        var outValue = new TypedValue();
        view.Context?.Theme?.ResolveAttribute(Android.Resource.Attribute.SelectableItemBackground, outValue, true);

        //var d = view.Context.Resources.GetDrawable(outValue.ResourceId); //Causes purple foreground
        var d = view.Context?.Resources?.GetDrawable(outValue.ResourceId, view.Context.Theme);
        //view.Foreground = d;
        view.Foreground = d;
    }

    protected override void OnDetached()
    {
        //throw new NotImplementedException();
    }

#elif __IOS__
    UIView? view;

    protected override void OnAttached()
    {
        // Get the iOS UIView corresponding to the Element that the effect is attached to
        view = Control == null ? Container : Control;
    }

    protected override void OnDetached()
    {
        //throw new NotImplementedException();
    }
#endif
}
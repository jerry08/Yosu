using UIKit;

namespace Plugin.MauiTouchEffect.Helpers;

static class XCT
{
    static bool? isiOS13OrNewer;

    internal static bool IsiOS13OrNewer => isiOS13OrNewer ??= UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
}
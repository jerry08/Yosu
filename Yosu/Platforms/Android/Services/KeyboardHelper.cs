using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Platform;

namespace Yosu.Platforms.Services;

public static class KeyboardHelper
{
    public static void HideKeyboard()
    {
        if (Platform.CurrentActivity?.CurrentFocus is not null)
            Platform.CurrentActivity?.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
    }
}

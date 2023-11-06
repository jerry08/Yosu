using UIKit;

namespace Yosu.Platforms.Services;

public static class KeyboardHelper
{
    public static void HideKeyboard()
    {
        UIApplication.SharedApplication?.KeyWindow.EndEditing(true);
    }
}

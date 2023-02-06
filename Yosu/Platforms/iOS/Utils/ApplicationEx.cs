using UIKit;

namespace Yosu.Utils;

public static class ApplicationEx
{
    /// <summary>
    /// Checks if the app it fully visible (active) and running. (Foreground)
    /// </summary>
    public static bool IsInForeground()
    {
        return UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active;
    }

    public static bool IsInBackground()
    {
        return UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background;
    }

    /// <summary>
    /// Checks if the app is running.
    /// </summary>
    public static bool IsRunning()
    {
        if (IsInForeground())
            return true;

        return UIApplication.SharedApplication.ApplicationState == UIApplicationState.Inactive;
    }
}
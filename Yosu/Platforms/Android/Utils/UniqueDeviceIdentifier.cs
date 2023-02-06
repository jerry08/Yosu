using Microsoft.Maui.ApplicationModel;

namespace Yosu.Utils;

internal class UniqueDeviceIdentifier
{
    public static string GetUniqueIdentifier()
    {
        return Android.Provider.Settings.Secure.GetString(Platform.AppContext.ContentResolver, Android.Provider.Settings.Secure.AndroidId) + "-Android" ?? "";
    }
}
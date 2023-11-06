using UIKit;

namespace Yosu.Utils;

internal class UniqueDeviceIdentifier
{
    public static string GetUniqueIdentifier()
    {
        return UIDevice.CurrentDevice.IdentifierForVendor.AsString().Replace("-", "") + "-iOS";
    }
}

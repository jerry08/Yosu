using Microsoft.Maui.Handlers;
using ASwitch = Google.Android.Material.MaterialSwitch.MaterialSwitch;

namespace Yosu.Handlers;

public class MaterialSwitchHandler : SwitchHandler
{
    protected override ASwitch CreatePlatformView()
    {
        return new ASwitch(Context);
    }
}
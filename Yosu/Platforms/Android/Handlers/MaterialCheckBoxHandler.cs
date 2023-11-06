using Microsoft.Maui.Handlers;
using ACheckBox = Google.Android.Material.CheckBox.MaterialCheckBox;

namespace Yosu.Handlers;

public class MaterialCheckBoxHandler : CheckBoxHandler
{
    protected override ACheckBox CreatePlatformView()
    {
        return new ACheckBox(Context);
    }
}

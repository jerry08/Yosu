using Microsoft.Maui.Controls;

namespace Yosu.Controls;

public class ContentView : Microsoft.Maui.Controls.ContentView
{
    public object Tag
    {
        get => (object)GetValue(TagProperty);
        set => SetValue(TagProperty, value);
    }

    public static readonly BindableProperty TagProperty = BindableProperty.Create(
        nameof(Tag),
        typeof(object),
        typeof(ProgressBar),
        false);
}
using Microsoft.Maui.Controls;
using Yosu.ViewModels.Settings;

namespace Yosu.Views.Settings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

using Yosu.ViewModels;

namespace Yosu.Views;

public partial class LoadingPopup
{
    public LoadingPopup(LoadingPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

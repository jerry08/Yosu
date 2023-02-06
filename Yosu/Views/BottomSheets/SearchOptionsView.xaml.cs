using Yosu.ViewModels;

namespace Yosu.Views.BottomSheets;

public partial class SearchOptionsView
{
    public SearchOptionsView(MainCollectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
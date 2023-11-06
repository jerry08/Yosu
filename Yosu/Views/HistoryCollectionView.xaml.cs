using Yosu.ViewModels;

namespace Yosu.Views;

public partial class HistoryCollectionView
{
    public HistoryCollectionView(HistoryCollectionViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}

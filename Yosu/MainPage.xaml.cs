using Microsoft.Maui.Controls;
using Yosu.ViewModels;

namespace Yosu.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainCollectionViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        viewModel.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectionMode))
            {
                if (viewModel.SelectionMode == SelectionMode.Multiple)
                {
                    downloadContent.Scale = 0.2;
                    downloadContent.IsVisible = true;
                    await downloadContent.ScaleTo(1, 100);
                }
                else
                {
                    await downloadContent.ScaleTo(0.2, 100);
                    downloadContent.IsVisible = false;
                }
            }
        };
    }
}
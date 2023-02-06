using Microsoft.Maui.Controls;
using Yosu.ViewModels;

namespace Yosu.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainCollectionViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
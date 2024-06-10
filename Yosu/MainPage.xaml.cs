using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Yosu.ViewModels;

namespace Yosu.Views;

public partial class MainPage : ContentPage
{
    private readonly IPopupService _popupService;

    public MainPage(MainCollectionViewModel viewModel, IPopupService popupService)
    {
        InitializeComponent();

        _popupService = popupService;

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

        //Loaded += MainPage_Loaded;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        App.RefreshCurrentPageBehaviors();
    }

    private async void MainPage_Loaded(object? sender, System.EventArgs e)
    {
        //var loadingPopupViewModel = new LoadingPopupViewModel();
        //var popup = new LoadingPopup(loadingPopupViewModel);
        //
        //Application.Current?.MainPage?.ShowPopup(popup);
        //
        //return;

        var p = await _popupService.ShowPopupAsync<LoadingPopupViewModel>(
            (vm) =>
            {
                vm.LoadingText = "test...";
            }
        );
    }
}

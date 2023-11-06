using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;
using Yosu.Models;
using Yosu.ViewModels;
using Yosu.ViewModels.Framework;
using Yosu.Views;
using Yosu.Views.Settings;

namespace Yosu;

public partial class AppShell : Shell
{
    private bool DoubleBackToExitPressedOnce { get; set; }

    public static event EventHandler<BackPressedEventArgs>? BackButtonPressed;

    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(HistoryCollectionView), typeof(HistoryCollectionView));
        Routing.RegisterRoute($"Settings/{nameof(SettingsPage)}", typeof(SettingsPage));

        Navigated += (s, e) =>
        {
            if (CurrentPage?.BindingContext is BaseViewModel viewModel)
            {
                if (!viewModel.IsInitialized)
                    viewModel.Load();

                viewModel.OnNavigated();
            }
        };

        Loaded += (s, e) =>
        {
            if (CurrentPage?.BindingContext is MainCollectionViewModel viewModel)
            {
                //viewModel.Entities = new(viewModel.Entities.ToList());
                viewModel.RefreshDownloadingItems();
            }

            if (App.IsOnline(false))
                App.CheckForUpdate();

            Connectivity.Current.ConnectivityChanged += (s, e) =>
            {
                if (App.IsOnline())
                    App.CheckForUpdate();
            };
        };
    }

    protected override bool OnBackButtonPressed()
    {
        var @event = new BackPressedEventArgs();
        BackButtonPressed?.Invoke(this, @event);

        if (@event.Cancelled)
            return true;

        if (
            Current.CurrentPage.BindingContext is CollectionViewModelBase viewModel
            && viewModel.SelectionMode != SelectionMode.None
        )
        {
            viewModel.SelectionMode = SelectionMode.None;
            viewModel.SelectedEntities.Clear();
            return true;
        }

        if (!Current.Navigation.NavigationStack.Any(x => x is not null))
        {
            if (DoubleBackToExitPressedOnce)
            {
                //Application.Current?.Quit();
                //return true;

                return base.OnBackButtonPressed();
            }

            DoubleBackToExitPressedOnce = true;

            Toast.Make("Please perform BACK again to Exit", ToastDuration.Short).Show();

            Task.Run(async () =>
            {
                await Task.Delay(2000);
                DoubleBackToExitPressedOnce = false;
            });

            return true;
        }

        return base.OnBackButtonPressed();
    }
}

using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using Yosu.ViewModels;
using Yosu.ViewModels.Components;

namespace Yosu.Views.BottomSheets;

public partial class DownloadMultipleYtOptionsView
{
    private readonly YoutubeViewModel _viewModel;
    private readonly List<YoutubeDownloadViewModel> _downloads;

    public DownloadMultipleYtOptionsView(
        YoutubeViewModel viewModel,
        List<YoutubeDownloadViewModel> downloads
    )
    {
        InitializeComponent();

        _viewModel = viewModel;
        _downloads = downloads;

        BindingContext = _viewModel;
    }

    [RelayCommand]
    void Download(object option)
    {
        _viewModel.Download(_downloads, option);
    }
}

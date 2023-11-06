using CommunityToolkit.Mvvm.ComponentModel;
using Yosu.ViewModels.Framework;

namespace Yosu.ViewModels;

public partial class LoadingPopupViewModel : BaseViewModel
{
    [ObservableProperty]
    string _loadingText = "";
}

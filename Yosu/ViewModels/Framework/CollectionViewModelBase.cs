using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Yosu.Common.Utils;

namespace Yosu.ViewModels.Framework;

public partial class CollectionViewModelBase : BaseViewModel
{
    [ObservableProperty]
    private string? _query;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _canLoadMore;

    /// <summary>
    /// Default limit is 50
    /// </summary>
    [ObservableProperty]
    private int _limit = 50;

    [ObservableProperty]
    private int _offset;

    [ObservableProperty]
    private int _total;

    [ObservableProperty]
    private SelectionMode _selectionMode;

    public ObservableRangeCollection<object> SelectedEntities { get; } = new();

    [ObservableProperty]
    private bool _isSelectAllChecked;

    [ObservableProperty]
    private object? _selectedEntity;

    public CollectionViewModelBase()
    {
        IsBusy = true;
        SelectionMode = SelectionMode.None;

        PropertyChanged += (s, e) => CanLoadMore = Offset < Total;
    }

    [RelayCommand]
    void EnableMultiSelect()
    {
        SelectionMode = SelectionMode.Multiple;
    }

    [RelayCommand]
    //void EnableMultiSelectWithParameter(T selectedItem)
    void EnableMultiSelectWithParameter(object selectedItem)
    {
        SelectionMode = SelectionMode.Multiple;

        SelectedEntity = selectedItem;

        if (!SelectedEntities.Contains(selectedItem))
            SelectedEntities.Add(selectedItem);
    }
}
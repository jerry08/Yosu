﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Yosu.Common.Utils;

namespace Yosu.ViewModels.Framework;

public partial class CollectionViewModel<T> : CollectionViewModelBase
    where T : class
{
    public ObservableRangeCollection<T> Entities { get; set; } = [];

    [RelayCommand]
    public virtual void LoadMore()
    {
        if (IsBusy)
            return;
        if (!CanLoadMore)
            return;

        if (Entities.Count == 0)
            return;

        IsLoading = true;
        LoadCore();
    }

    public void Push(IEnumerable<T> entities)
    {
        // Convert to list to prevent multi-select bugs
        var list = entities.ToList();

        if (IsRefreshing)
        {
            Entities.ReplaceRange(list);
        }
        else
        {
            Entities.Clear();
            Entities.AddRange(list);
        }

        IsBusy = false;
        IsRefreshing = false;
        IsLoading = false;
    }

    [RelayCommand]
    public virtual async Task Refresh()
    {
        if (IsBusy)
            return;

        if (!await IsOnline())
        {
            IsRefreshing = false;
            return;
        }

        if (!IsRefreshing)
            IsBusy = true;

        //await Task.Delay(100);

        //Entities.Clear();
        Offset = 0;
        LoadCore();
    }

    [RelayCommand]
    async Task QueryChanged()
    {
        if (!await IsOnline())
            return;

        IsBusy = true;
        Offset = 0;
        //Entities.Clear();
        LoadCore();
    }

    [RelayCommand]
    void SelectionChanged()
    {
        if (Entities.Count == 0)
            return;

        IsSelectAllChecked = SelectedEntities.Count == Entities.Count;
    }

    [RelayCommand]
    void SelectOrUnselectAll()
    {
        if (Entities.Count == 0)
            return;

        if (IsSelectAllChecked)
            SelectedEntities.ReplaceRange(Entities);
        else
            SelectedEntities.Clear();
    }
}

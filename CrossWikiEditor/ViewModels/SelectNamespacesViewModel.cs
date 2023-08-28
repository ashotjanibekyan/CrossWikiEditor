using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Utils;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public sealed partial class SelectNamespacesViewModel(List<WikiNamespace> namespaces) : ViewModelBase
{
    [RelayCommand]
    private void Select(IDialog dialog)
    {
        dialog.Close(Result<int[]>.Success(Namespaces.Where(n => n.IsChecked).Select(n => n.Id).ToArray()));
    }

    partial void OnIsAllSelectedChanged(bool value)
    {
        Namespaces = Namespaces
            .ToList()
            .Select(x => new WikiNamespace(x.Id, x.Name, value))
            .ToObservableCollection();
    }

    [ObservableProperty] private ObservableCollection<WikiNamespace> _namespaces = namespaces.Where(x => x.Id >= 0).ToObservableCollection();
    [ObservableProperty] private bool _isAllSelected;
}
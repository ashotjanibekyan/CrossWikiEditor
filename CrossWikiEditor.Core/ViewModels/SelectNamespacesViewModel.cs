using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class SelectNamespacesViewModel(List<WikiNamespace> namespaces) : ViewModelBase
{
    [RelayCommand]
    private void Select(IDialog dialog)
    {
        dialog.Close(Enumerable.Where<WikiNamespace>(Namespaces, n => n.IsChecked).Select(n => n.Id).ToArray());
    }

    partial void OnIsAllSelectedChanged(bool value)
    {
        Namespaces = Enumerable
            .ToList<WikiNamespace>(Namespaces)
            .Select(x => new WikiNamespace(x.Id, x.Name, value))
            .ToObservableCollection();
    }

    [ObservableProperty] private ObservableCollection<WikiNamespace> _namespaces = namespaces.Where(x => x.Id >= 0).ToObservableCollection();
    [ObservableProperty] private bool _isAllSelected;
}
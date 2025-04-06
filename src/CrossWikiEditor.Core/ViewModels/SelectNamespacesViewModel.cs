using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class SelectNamespacesViewModel : ViewModelBase
{
    public SelectNamespacesViewModel(List<WikiNamespace> namespaces, bool isMultiselect = true)
    {
        Namespaces = namespaces.Where(x => x.Id >= 0).ToObservableCollection();
        IsMultiselect = isMultiselect;
    }

    [ObservableProperty]
    public partial ObservableCollection<WikiNamespace> Namespaces { get; set; }

    [ObservableProperty] public partial bool IsAllSelected { get; set; }

    [ObservableProperty] public partial bool IsMultiselect { get; set; }

    [RelayCommand]
    private void Select(IDialog dialog)
    {
        dialog.Close(Namespaces.Where(n => n.IsChecked).Select(n => n.Id).ToArray());
    }

    partial void OnIsAllSelectedChanged(bool value)
    {
        Namespaces = Namespaces
            .ToList()
            .Select(x => new WikiNamespace(x.Id, x.Name, value))
            .ToObservableCollection();
    }
}
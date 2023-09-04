using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class SpecialPageListProviderSelectorViewModel(List<ISpecialPageListProvider> listProviders, List<WikiNamespace> namespaces) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<ISpecialPageListProvider> _listProviders = listProviders.ToObservableCollection();
    [ObservableProperty] private ISpecialPageListProvider? _selectedListProvider = listProviders.FirstOrDefault();
    [ObservableProperty] private ObservableCollection<WikiNamespace> _namespaces = namespaces.ToObservableCollection();
    [ObservableProperty] private WikiNamespace? _selectedNamespace = namespaces.FirstOrDefault();

    [RelayCommand]
    private void Ok(IDialog dialog)
    {
        if (SelectedNamespace is null || SelectedListProvider is null)
        {
            return;
        }
        SelectedListProvider.NamespaceId = SelectedNamespace.Id;
        dialog.Close(SelectedListProvider);
    }

    [RelayCommand]
    private void Cancel(IDialog dialog)
    {
        dialog.Close(null);
    }
}

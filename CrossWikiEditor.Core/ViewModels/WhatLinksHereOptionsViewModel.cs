using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class WhatLinksHereOptionsViewModel(List<WikiNamespace> namespaces) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<WikiNamespace> _namespaces = namespaces.ToObservableCollection();
    [ObservableProperty] private bool _isAllNamespacesChecked;
    [ObservableProperty] private int _selectedRedirectFilter = 0;
    [ObservableProperty] private bool _includeRedirects;
    partial void OnIsAllNamespacesCheckedChanged(bool value)
    {
        Namespaces = Namespaces
            .Select<WikiNamespace, WikiNamespace>(x => new WikiNamespace(x.Id, x.Name, value)).ToObservableCollection();
    }

    [RelayCommand]
    private void Ok(IDialog dialog)
    {
        var result = new WhatLinksHereOptions(Namespaces.Where<WikiNamespace>(x => x.IsChecked).Select(x => x.Id).ToArray(), IncludeRedirects,
            (RedirectFilter) SelectedRedirectFilter);
        dialog.Close(result);
    }
}
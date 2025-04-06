using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.Utils.Extensions;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class SelectNamespacesAndRedirectFilterViewModel : ViewModelBase
{
    public SelectNamespacesAndRedirectFilterViewModel(List<WikiNamespace> namespaces)
    {
        Namespaces = namespaces.ToObservableCollection();
    }

    [ObservableProperty] public partial ObservableCollection<WikiNamespace> Namespaces { get; set; }

    [ObservableProperty] public partial bool IsAllNamespacesChecked { get; set; }

    [ObservableProperty] public partial int SelectedRedirectFilter { get; set; } = 0;

    [ObservableProperty] public partial bool IncludeRedirects { get; set; }

    [ObservableProperty] public partial bool IsIncludeRedirectsVisible { get; set; }

    partial void OnIsAllNamespacesCheckedChanged(bool value)
    {
        Namespaces = Namespaces
            .Select<WikiNamespace, WikiNamespace>(x => new WikiNamespace(x.Id, x.Name, value)).ToObservableCollection();
    }

    [RelayCommand]
    private void Ok(IDialog dialog)
    {
        var result = new NamespacesAndRedirectFilterOptions(Namespaces.Where(x => x.IsChecked).Select(x => x.Id).ToArray(), IncludeRedirects,
            (RedirectFilter) SelectedRedirectFilter);
        dialog.Close(result);
    }
}
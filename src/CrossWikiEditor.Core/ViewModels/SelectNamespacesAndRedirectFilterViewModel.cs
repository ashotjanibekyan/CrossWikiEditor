namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class SelectNamespacesAndRedirectFilterViewModel(List<WikiNamespace> namespaces) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<WikiNamespace> _namespaces = namespaces.ToObservableCollection();
    [ObservableProperty] private bool _isAllNamespacesChecked;
    [ObservableProperty] private int _selectedRedirectFilter = 0;
    [ObservableProperty] private bool _includeRedirects;
    [ObservableProperty] private bool _isIncludeRedirectsVisible;

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
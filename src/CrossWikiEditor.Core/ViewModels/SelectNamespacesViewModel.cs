namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class SelectNamespacesViewModel(List<WikiNamespace> namespaces, bool isMultiselect = true) : ViewModelBase
{
    [ObservableProperty]
    public partial ObservableCollection<WikiNamespace> Namespaces { get; set; } = namespaces.Where(x => x.Id >= 0).ToObservableCollection();

    [ObservableProperty] public partial bool IsAllSelected { get; set; }

    [ObservableProperty] public partial bool IsMultiselect { get; set; } = isMultiselect;

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
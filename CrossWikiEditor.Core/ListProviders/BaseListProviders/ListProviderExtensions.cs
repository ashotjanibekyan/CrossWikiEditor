using CrossWikiEditor.Core.Services;

namespace CrossWikiEditor.Core.ListProviders.BaseListProviders;

public static class ListProviderExtensions
{
    public static async Task<int[]?> GetNamespaces(this INeedNamespacesListProvider _, bool isMultiselect, IDialogService dialogService,
        IViewModelFactory viewModelFactory)
    {
        return await dialogService.ShowDialog<int[]?>(await viewModelFactory.GetSelectNamespacesViewModel(isMultiselect));
    }
}
namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllPagesWithPrefixListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService,
    IViewModelFactory viewModelFactory) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string Title => "All Pages with prefix (Prefixindex)";
    public override string ParamTitle => "Prefix";
    public override bool CanMake => _namespaces is {Length: 1};

    public async Task GetAdditionalParams()
    {
        _namespaces = await this.GetNamespaces(false, DialogService, viewModelFactory);
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllPagesWithPrefix(settingsService.CurrentApiUrl, Param, _namespaces![0], limit);
    }
}
namespace CrossWikiEditor.Core.ListProviders.BaseListProviders;

public abstract class AllPagesListProviderBase(
    IDialogService dialogService,
    IPageService pageService,
    IViewModelFactory viewModelFactory,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string ParamTitle => "Start from";
    public override bool CanMake => _namespaces is {Length: 1};

    public async Task GetAdditionalParams()
    {
        _namespaces = await this.GetNamespaces(false, DialogService, viewModelFactory);
    }

    protected async Task<Result<List<WikiPageModel>>> MakeListBase(int limit, PropertyFilterOption redirectsFilter,
        PropertyFilterOption langLinksFilter)
    {
        return await pageService.GetAllPages(settingsService.CurrentApiUrl, Param, _namespaces![0],
            redirectsFilter, langLinksFilter, limit);
    }
}
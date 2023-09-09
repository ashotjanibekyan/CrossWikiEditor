namespace CrossWikiEditor.Core.ListProviders.BaseListProviders;

public abstract class AllPagesListProviderBase(
    IDialogService dialogService,
    IPageService pageService,
    IViewModelFactory viewModelFactory,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string ParamTitle => "Start from";
    public override bool CanMake => _namespaces is {Length: 1};
    public async Task GetAdditionalParams() => _namespaces = await this.GetNamespaces(isMultiselect: false, dialogService, viewModelFactory);

    protected async Task<Result<List<WikiPageModel>>> MakeListBase(int limit, PropertyFilterOption redirectsFilter,
        PropertyFilterOption langLinksFilter)
    {
        return await pageService.GetAllPages(userPreferencesService.CurrentApiUrl, Param, _namespaces!.First(),
            redirectsFilter: redirectsFilter, langLinksFilter: langLinksFilter, limit);
    }
}
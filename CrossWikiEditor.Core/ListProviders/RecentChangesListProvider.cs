namespace CrossWikiEditor.Core.ListProviders;

public sealed class RecentChangesListProvider(IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService,
    IViewModelFactory viewModelFactory) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string Title => "Recent Changes";
    public override string ParamTitle => "";
    public override bool CanMake => _namespaces is {Length: > 0};
    
    public async Task GetAdditionalParams()
    {
        _namespaces = await this.GetNamespaces(true, DialogService, viewModelFactory);
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetRecentlyChangedPages(userPreferencesService.CurrentApiUrl, _namespaces, limit);
}
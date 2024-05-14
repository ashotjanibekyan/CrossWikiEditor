namespace CrossWikiEditor.Core.ListProviders;

public sealed class NewPagesListProvider(IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService,
    IViewModelFactory viewModelFactory) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string Title => "New pages";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _namespaces is {Length: > 0};
    
    public async Task GetAdditionalParams() => _namespaces = await this.GetNamespaces(true, DialogService, viewModelFactory);

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetNewPages(settingsService.CurrentApiUrl, _namespaces!, limit);
}
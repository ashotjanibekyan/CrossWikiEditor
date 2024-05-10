namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllRedirectsListProvider(IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService,
    IViewModelFactory viewModelFactory) : AllPagesListProviderBase(dialogService, pageService, viewModelFactory, userPreferencesService)
{
    public override string Title => "All Redirects";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await MakeListBase(limit, PropertyFilterOption.WithProperty, PropertyFilterOption.Disable);
}
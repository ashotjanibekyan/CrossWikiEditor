namespace CrossWikiEditor.Core.ListProviders;

public class PagesWithoutLanguageLinksNoRedirectsListProvider(
    IDialogService dialogService, 
    IPageService pageService,
    IViewModelFactory viewModelFactory, 
    IUserPreferencesService userPreferencesService) : AllPagesListProviderBase(dialogService, pageService, viewModelFactory, userPreferencesService)
{
    public override string Title => "Pages without language links (no redirects)";
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await MakeListBase(limit, PropertyFilterOption.WithoutProperty, PropertyFilterOption.WithoutProperty);
}
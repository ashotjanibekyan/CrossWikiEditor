namespace CrossWikiEditor.Core.ListProviders;

public class PagesWithoutLanguageLinksListProvider(IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService,
    IViewModelFactory viewModelFactory) : AllPagesListProviderBase(dialogService, pageService, viewModelFactory, userPreferencesService)
{
    public override string Title => "Pages without language links";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await MakeListBase(limit, PropertyFilterOption.Disable, PropertyFilterOption.WithoutProperty);
}
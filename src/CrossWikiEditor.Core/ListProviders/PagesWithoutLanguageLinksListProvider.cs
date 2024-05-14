namespace CrossWikiEditor.Core.ListProviders;

public sealed class PagesWithoutLanguageLinksListProvider(IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService,
    IViewModelFactory viewModelFactory) : AllPagesListProviderBase(dialogService, pageService, viewModelFactory, settingsService)
{
    public override string Title => "Pages without language links";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await MakeListBase(limit, PropertyFilterOption.Disable, PropertyFilterOption.WithoutProperty);
}
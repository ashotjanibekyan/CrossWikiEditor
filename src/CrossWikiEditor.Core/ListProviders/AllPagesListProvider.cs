namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllPagesListProvider(IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService,
    IViewModelFactory viewModelFactory) : AllPagesListProviderBase(dialogService, pageService, viewModelFactory, settingsService)
{
    public override string Title => "All Pages";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await MakeListBase(limit, PropertyFilterOption.Disable, PropertyFilterOption.Disable);
}
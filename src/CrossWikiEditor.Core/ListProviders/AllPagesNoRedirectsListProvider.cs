namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllPagesNoRedirectsListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService,
    IViewModelFactory viewModelFactory) : AllPagesListProviderBase(dialogService, pageService, viewModelFactory, settingsService)
{
    public override string Title => "All Pages (no redirects)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await MakeListBase(limit, PropertyFilterOption.WithoutProperty, PropertyFilterOption.Disable);
    }
}
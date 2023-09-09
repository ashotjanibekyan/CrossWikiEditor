namespace CrossWikiEditor.Core.ListProviders;

public sealed class LinkSearchListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Link search";
    public override string ParamTitle => "URL";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.LinkSearch(userPreferencesService.CurrentApiUrl, Param, limit);
}
namespace CrossWikiEditor.Core.ListProviders;

public sealed class LinkSearchListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Link search";
    public override string ParamTitle => "URL";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.LinkSearch(settingsService.CurrentApiUrl, Param, limit);
    }
}
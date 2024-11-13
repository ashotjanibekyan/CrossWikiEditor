namespace CrossWikiEditor.Core.ListProviders;

public sealed class WikiSearchInTitleListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Wiki search (title)";
    public override string ParamTitle => "Wiki search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.WikiSearch(settingsService.CurrentApiUrl, $"intitle:{Param}", [0], limit);
    }
}
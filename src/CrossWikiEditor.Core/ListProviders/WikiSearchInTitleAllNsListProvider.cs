namespace CrossWikiEditor.Core.ListProviders;

public sealed class WikiSearchInTitleAllNsListProvider(IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Wiki search (title) (all NS)";
    public override string ParamTitle => "Wiki search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.WikiSearch(settingsService.CurrentApiUrl, $"intitle:{Param}", [], limit);
}
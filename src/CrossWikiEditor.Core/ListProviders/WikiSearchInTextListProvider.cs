namespace CrossWikiEditor.Core.ListProviders;

public sealed class WikiSearchInTextListProvider(IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Wiki search (text)";
    public override string ParamTitle => "Wiki search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.WikiSearch(settingsService.CurrentApiUrl, Param, new[] {0}, limit);
}
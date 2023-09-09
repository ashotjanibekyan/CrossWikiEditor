namespace CrossWikiEditor.Core.ListProviders;

public sealed class WikiSearchInTitleListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Wiki search (title)";
    public override string ParamTitle => "Wiki search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.WikiSearch(userPreferencesService.CurrentApiUrl, $"intitle:{Param}", new[] {0}, limit);
}
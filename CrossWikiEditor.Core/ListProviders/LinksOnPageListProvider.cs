namespace CrossWikiEditor.Core.ListProviders;

public class LinksOnPageListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Links on page";
    public override string ParamTitle => "Links on";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.LinksOnPage(userPreferencesService.CurrentApiUrl, Param, limit);
}
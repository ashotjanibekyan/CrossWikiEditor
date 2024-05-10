namespace CrossWikiEditor.Core.ListProviders;

public class LinksOnPageListProvider(IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Links on page";
    public override string ParamTitle => "Links on";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.LinksOnPage(userPreferencesService.CurrentApiUrl, Param, limit);
}
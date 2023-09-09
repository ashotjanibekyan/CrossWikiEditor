namespace CrossWikiEditor.Core.ListProviders;

public sealed class PagesWithPropListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Pages with a page property";
    public override string ParamTitle => "Property name";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetPagesWithProp(userPreferencesService.CurrentApiUrl, Param, limit);
}
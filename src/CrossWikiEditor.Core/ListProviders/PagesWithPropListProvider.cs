namespace CrossWikiEditor.Core.ListProviders;

public sealed class PagesWithPropListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Pages with a page property";
    public override string ParamTitle => "Property name";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetPagesWithProp(settingsService.CurrentApiUrl, Param, limit);
    }
}
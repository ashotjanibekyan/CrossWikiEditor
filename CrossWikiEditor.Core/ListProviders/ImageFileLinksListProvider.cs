namespace CrossWikiEditor.Core.ListProviders;

public sealed class ImageFileLinksListProvider(
    IPageService pageService,
    IUserPreferencesService userPreferencesService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Image file links";
    public override string ParamTitle => "File";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetPagesByFileUsage(userPreferencesService.CurrentApiUrl, Param, limit);
}
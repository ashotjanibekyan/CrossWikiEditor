namespace CrossWikiEditor.Core.ListProviders;

public sealed class FilesOnPageListProvider(IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Files on page";
    public override string ParamTitle => "Files on";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.FilesOnPage(userPreferencesService.CurrentApiUrl, Param, limit);
}
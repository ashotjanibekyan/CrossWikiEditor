namespace CrossWikiEditor.Core.ListProviders;

public sealed class FilesOnPageListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Files on page";
    public override string ParamTitle => "Files on";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.FilesOnPage(settingsService.CurrentApiUrl, Param, limit);
    }
}
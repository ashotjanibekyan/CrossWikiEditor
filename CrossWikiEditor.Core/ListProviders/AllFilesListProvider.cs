namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllFilesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "All Files";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetAllFiles(userPreferencesService.CurrentApiUrl, Param, limit);
}
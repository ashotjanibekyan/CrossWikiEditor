namespace CrossWikiEditor.Core.ListProviders;

public sealed class DisambiguationPagesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Disambiguation pages";
    public override string ParamTitle => "";
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetPagesWithProp(userPreferencesService.CurrentApiUrl, "disambiguation", limit);
}
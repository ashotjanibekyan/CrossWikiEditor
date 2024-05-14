namespace CrossWikiEditor.Core.ListProviders;

public sealed class LinksOnPageRedListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService)
    : LinksOnPageListProvider(dialogService, pageService, settingsService)
{
    public override string Title => "Links on page (only redlinks)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        Result<List<WikiPageModel>> result = await base.MakeList(limit);
        if (result is not {IsSuccessful: true, Value: not null})
        {
            return result;
        }

        bool[] existsTable = await Task.WhenAll(result.Value.Select(p => p.Exists()));
        return result.Value.Where((_, index) => !existsTable[index]).ToList();
    }
}
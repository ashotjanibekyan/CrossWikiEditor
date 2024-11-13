namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryRecursiveListProvider(
    ICategoryService categoryService,
    IDialogService dialogService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Category (recursive)";
    public override string ParamTitle => "Category";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await categoryService.GetPagesOfCategory(settingsService.CurrentApiUrl, Param, limit, int.MaxValue);
    }
}
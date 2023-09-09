namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryRecursiveListProvider(
    IUserPreferencesService userPreferencesService,
    ICategoryService categoryService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Category (recursive)";
    public override string ParamTitle => "Category";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetPagesOfCategory(userPreferencesService.CurrentApiUrl, Param, limit, Int32.MaxValue);
}
namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryListProvider(
    IUserPreferencesService userPreferencesService,
    ICategoryService categoryService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Category";
    public override string ParamTitle => "Category";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetPagesOfCategory(userPreferencesService.CurrentApiUrl, Param, limit);
}
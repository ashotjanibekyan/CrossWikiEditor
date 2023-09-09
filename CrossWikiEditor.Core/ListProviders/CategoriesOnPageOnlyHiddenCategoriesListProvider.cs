namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoriesOnPageOnlyHiddenCategoriesListProvider(
        IUserPreferencesService userPreferencesService,
        ICategoryService categoryService,
        IDialogService dialogService)
    : CategoriesOnPageListProvider(userPreferencesService, categoryService, dialogService)
{
    public override string Title => "Categories on page (only hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetCategoriesOf(userPreferencesService.CurrentApiUrl, Param, limit, onlyHidden: true);
}
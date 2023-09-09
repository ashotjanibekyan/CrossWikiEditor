namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoriesOnPageOnlyHiddenCategoriesListProvider(ICategoryService categoryService,
        IDialogService dialogService, IUserPreferencesService userPreferencesService)
    : CategoriesOnPageListProvider(categoryService, dialogService, userPreferencesService)
{
    public override string Title => "Categories on page (only hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetCategoriesOf(userPreferencesService.CurrentApiUrl, Param, limit, onlyHidden: true);
}
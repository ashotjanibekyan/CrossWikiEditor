namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoriesOnPageNoHiddenCategoriesListProvider(
        IUserPreferencesService userPreferencesService,
        ICategoryService categoryService,
        IDialogService dialogService) 
    : CategoriesOnPageListProvider(userPreferencesService, categoryService, dialogService)
{
    public override string Title => "Categories on page (no hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetCategoriesOf(userPreferencesService.CurrentApiUrl, Param, limit, includeHidden: false);
}
namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoriesOnPageNoHiddenCategoriesListProvider(ICategoryService categoryService,
        IDialogService dialogService, IUserPreferencesService userPreferencesService) 
    : CategoriesOnPageListProvider(categoryService, dialogService, userPreferencesService)
{
    public override string Title => "Categories on page (no hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetCategoriesOf(userPreferencesService.CurrentApiUrl, Param, limit, includeHidden: false);
}
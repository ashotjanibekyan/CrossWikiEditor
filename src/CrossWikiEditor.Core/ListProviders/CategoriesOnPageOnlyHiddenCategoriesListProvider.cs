namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoriesOnPageOnlyHiddenCategoriesListProvider(
    ICategoryService categoryService,
    IDialogService dialogService,
    ISettingsService settingsService) : CategoriesOnPageListProvider(categoryService, dialogService, settingsService)
{
    public override string Title => "Categories on page (only hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await CategoryService.GetCategoriesOf(SettingsService.CurrentApiUrl, Param, limit, onlyHidden: true);
    }
}
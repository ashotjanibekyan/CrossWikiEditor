namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoriesOnPageNoHiddenCategoriesListProvider(
    ICategoryService categoryService,
    IDialogService dialogService,
    ISettingsService settingsService) : CategoriesOnPageListProvider(categoryService, dialogService, settingsService)
{
    public override string Title => "Categories on page (no hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await CategoryService.GetCategoriesOf(SettingsService.CurrentApiUrl, Param, limit, includeHidden: false);
}
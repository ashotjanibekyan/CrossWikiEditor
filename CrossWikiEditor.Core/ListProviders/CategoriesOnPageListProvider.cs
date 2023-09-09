namespace CrossWikiEditor.Core.ListProviders;

public class CategoriesOnPageListProvider(
    IUserPreferencesService userPreferencesService,
    ICategoryService categoryService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Categories on page";
    public override string ParamTitle => "Page";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetCategoriesOf(userPreferencesService.CurrentApiUrl, Param, limit);
}
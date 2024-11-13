namespace CrossWikiEditor.Core.ListProviders;

public class CategoriesOnPageListProvider(
    ICategoryService categoryService,
    IDialogService dialogService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    protected ISettingsService SettingsService => settingsService;
    protected ICategoryService CategoryService => categoryService;
    public override string Title => "Categories on page";
    public override string ParamTitle => "Page";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await categoryService.GetCategoriesOf(settingsService.CurrentApiUrl, Param, limit);
    }
}
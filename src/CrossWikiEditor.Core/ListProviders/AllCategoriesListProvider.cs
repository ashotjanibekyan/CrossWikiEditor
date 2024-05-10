namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllCategoriesListProvider(ICategoryService categoryService,
    IDialogService dialogService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "All Categories";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetAllCategories(userPreferencesService.CurrentApiUrl, Param, limit);
}
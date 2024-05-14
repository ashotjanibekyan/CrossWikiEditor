namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryListProvider(ICategoryService categoryService,
    IDialogService dialogService, ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Category";
    public override string ParamTitle => "Category";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetPagesOfCategory(settingsService.CurrentApiUrl, Param, limit);
}
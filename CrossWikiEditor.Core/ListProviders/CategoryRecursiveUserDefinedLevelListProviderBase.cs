namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryRecursiveUserDefinedLevelListProviderBase(
        ICategoryService categoryService,
        IUserPreferencesService userPreferencesService,
        IDialogService dialogService)
    : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private int? recursionLevel;

    public override string Title => "Category (recursive user defined level)";
    public override string ParamTitle => "Category";
    public override bool CanMake => recursionLevel is not null && !string.IsNullOrWhiteSpace(Param);

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        if (recursionLevel is null)
        {
            return Result<List<WikiPageModel>>.Failure("Please select recursive level.");
        }

        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        Result<List<WikiPageModel>> result = await categoryService.GetPagesOfCategory(userPrefs.UrlApi(), Param, limit, (int) recursionLevel);
        recursionLevel = null;
        return result;
    }

    public async Task GetAdditionalParams()
    {
        int? result = await dialogService.ShowDialog<int?>(new PromptViewModel("Number", "Recursion depth: ")
        {
            IsNumeric = true,
            Value = recursionLevel ?? 1
        });
        if (result is not null)
        {
            recursionLevel = result;
        }
    }
}
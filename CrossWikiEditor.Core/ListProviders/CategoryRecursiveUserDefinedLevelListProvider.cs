namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryRecursiveUserDefinedLevelListProvider(
        ICategoryService categoryService,
        IUserPreferencesService userPreferencesService,
        IDialogService dialogService)
    : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private int? _recursionLevel;

    public override string Title => "Category (recursive user defined level)";
    public override string ParamTitle => "Category";
    public override bool CanMake => _recursionLevel is not null && !string.IsNullOrWhiteSpace(Param);

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        if (_recursionLevel is null)
        {
            return Result<List<WikiPageModel>>.Failure("Please select recursive level.");
        }

        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        Result<List<WikiPageModel>> result = await categoryService.GetPagesOfCategory(userPrefs.UrlApi(), Param, limit, (int) _recursionLevel);
        _recursionLevel = null;
        return result;
    }

    public async Task GetAdditionalParams()
    {
        int? result = await dialogService.ShowDialog<int?>(new PromptViewModel("Number", "Recursion depth: ")
        {
            IsNumeric = true,
            Value = _recursionLevel ?? 1
        });
        if (result is not null)
        {
            _recursionLevel = result;
        }
    }
}
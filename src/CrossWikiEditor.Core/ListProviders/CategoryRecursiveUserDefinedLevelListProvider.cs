namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryRecursiveUserDefinedLevelListProvider(
    ICategoryService categoryService,
    IDialogService dialogService,
    ISettingsService settingsService)
    : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private int? _recursionLevel;

    public override string Title => "Category (recursive user defined level)";
    public override string ParamTitle => "Category";
    public override bool CanMake => _recursionLevel is not null && !string.IsNullOrWhiteSpace(Param);

    public async Task GetAdditionalParams()
    {
        int? result = await DialogService.ShowDialog<int?>(new PromptViewModel("Number", "Recursion depth: ")
        {
            IsNumeric = true,
            Value = _recursionLevel ?? 1
        });
        if (result is not null)
        {
            _recursionLevel = result;
        }
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        if (_recursionLevel is null)
        {
            return new Exception("Please select recursive level.");
        }

        UserSettings userSettings = settingsService.GetCurrentSettings();
        Result<List<WikiPageModel>> result = await categoryService.GetPagesOfCategory(userSettings.GetApiUrl(), Param, limit, (int) _recursionLevel);
        _recursionLevel = null;
        return result;
    }
}
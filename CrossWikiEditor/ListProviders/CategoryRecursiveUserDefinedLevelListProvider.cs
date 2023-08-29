using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Settings;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.ListProviders;

public class CategoryRecursiveUserDefinedLevelListProvider(IPageService pageService,
        IUserPreferencesService userPreferencesService,
        IDialogService dialogService)
    : INeedAdditionalParamsListProvider
{
    private int? recursionLevel;

    public string Title => "Category (recursive user defined level)";
    public string ParamTitle => "Category";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => recursionLevel is not null && !string.IsNullOrWhiteSpace(Param);

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        if (recursionLevel is null)
        {
            return Result<List<WikiPageModel>>.Failure("Please select recursive level.");
        }

        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        Result<List<WikiPageModel>> result = await pageService.GetPagesOfCategory(userPrefs.UrlApi(), Param, (int) recursionLevel);
        recursionLevel = null;
        return result;
    }

    public async Task GetAdditionalParams()
    {
        int result = await dialogService.ShowDialog<int>(new PromptViewModel("Number", "Recursion depth: ")
        {
            IsNumeric = true,
            Value = recursionLevel ?? 1
        });
        if (result != -1)
        {
            recursionLevel = result;
        }
    }
}
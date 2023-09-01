using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Settings;
using CrossWikiEditor.Utils;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.ListProviders;

public class CategoryRecursiveUserDefinedLevelListProviderBase(
        IPageService pageService,
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
        Result<List<WikiPageModel>> result = await pageService.GetPagesOfCategory(userPrefs.UrlApi(), Param, limit, (int) recursionLevel);
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
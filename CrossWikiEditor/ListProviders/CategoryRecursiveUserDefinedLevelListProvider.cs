using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.ViewModels;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor.ListProviders;

public class CategoryRecursiveUserDefinedLevelListProvider : IListProvider
{
    private readonly IPageService _pageService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IDialogService _dialogService;
    private int? recursionLevel;
    
    public CategoryRecursiveUserDefinedLevelListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService,
        IDialogService dialogService)
    {
        _pageService = pageService;
        _userPreferencesService = userPreferencesService;
        _dialogService = dialogService;
    }
    
    public string Title => "Category (recursive user defined level)";
    public string ParamTitle => "Category";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => recursionLevel is not null && !string.IsNullOrWhiteSpace(Param);
    public bool NeedsAdditionalParams => true;
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        if (recursionLevel is null)
        {
            return Result<List<WikiPageModel>>.Failure("Please select recursive level.");
        }

        UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
        return await _pageService.GetPagesOfCategory(userPrefs.UrlApi(), Param, recursive: (int)recursionLevel);

    }

    public async Task GetAdditionalParams()
    {
        var result = await _dialogService.ShowDialog<int>(new PromptViewModel("Number", "Recursion depth: ")
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class CategoryRecursiveListProvider : IListProvider
{
    private readonly IPageService _pageService;
    private readonly IUserPreferencesService _userPreferencesService;

    public CategoryRecursiveListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService)
    {
        _pageService = pageService;
        _userPreferencesService = userPreferencesService;
    }
    
    public string Title => "Category (recursive)";
    public string ParamTitle => "Category";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public bool NeedsAdditionalParams => false;

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
        return await _pageService.GetPagesOfCategory(userPrefs.UrlApi(), Param, recursive: Int32.MaxValue);
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }
}
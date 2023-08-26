﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor.ListProviders;

public class CategoryRecursive1LevelListProvider : IListProvider
{
    private readonly IPageService _pageService;
    private readonly IUserPreferencesService _userPreferencesService;
    
    public CategoryRecursive1LevelListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService)
    {
        _pageService = pageService;
        _userPreferencesService = userPreferencesService;
    }
    
    public string Title => "Category (recursive 1 level)";
    public string ParamTitle => "Category";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public bool NeedsAdditionalParams => false;
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
        return await _pageService.GetPagesOfCategory(userPrefs.UrlApi(), Param, recursive: 1);
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }
}
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

public class WikiSearchInTitleAllNsListProvider(IUserPreferencesService userPreferencesService, IPageService pageService) : IListProvider
{
    public string Title => "Wiki search (title) (all NS)";
    public string ParamTitle => "Wiki search";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        string apiRoot = userPreferencesService.GetCurrentPref().UrlApi();
        return await pageService.WikiSearch(apiRoot, $"intitle:{Param}", null);
    }
}
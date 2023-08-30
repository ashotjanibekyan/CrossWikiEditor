﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class WhatTranscludesHereAllNsListProvider(IUserPreferencesService userPreferencesService, IPageService pageService) : IListProvider
{
    public string Title => "What transcludes page (all NS)";
    public string ParamTitle => "What embeds";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        string apiRoot = userPreferencesService.GetCurrentPref().UrlApi();
        return await pageService.GetTransclusionsOf(apiRoot, Param, null);
    }
}
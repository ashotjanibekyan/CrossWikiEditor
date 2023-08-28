﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;
public sealed class TransclusionsOnPageListProvider(IUserPreferencesService userPreferencesService, IPageService pageService) : IListProvider
{
    public string Title => "Transclusions on page";

    public string ParamTitle => "Transclusions on";

    public string Param { get; set; } = string.Empty;

    public bool CanMake => !string.IsNullOrEmpty(Param);

    public bool NeedsAdditionalParams => false;

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await pageService.GetTransclusionsOn(userPreferencesService.GetCurrentPref().UrlApi(), Param);
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }

}

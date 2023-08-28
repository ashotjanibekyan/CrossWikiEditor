﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class RandomListProvider(IPageService pageService,
        IDialogService dialogService,
        IViewModelFactory viewModelFactory,
        IUserPreferencesService userPreferencesService)
    : IListProvider
{
    private int[]? _namespaces;

    public string Title => "Random pages";
    public string ParamTitle => string.Empty;
    public string Param { get; set; } = string.Empty;
    public bool CanMake => _namespaces is not null;
    public bool NeedsAdditionalParams => true;

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        Result<List<WikiPageModel>> result = await pageService.GetRandomPages(userPreferencesService.GetCurrentPref().UrlApi(), 500, _namespaces);
        _namespaces = null;
        return result;
    }

    public async Task GetAdditionalParams()
    {
        Result<int[]> result = await dialogService.ShowDialog<Result<int[]>>(await viewModelFactory.GetSelectNamespacesViewModel());
        if (result is {IsSuccessful: true, Value: not null})
        {
            _namespaces = result.Value;
        }
    }
}
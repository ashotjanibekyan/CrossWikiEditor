﻿using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class AllPagesWithPrefixListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IViewModelFactory viewModelFactory,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespace;
    public override string Title => "All Pages with prefix (Prefixindex)";
    public override string ParamTitle => "Prefix";
    public override bool CanMake => _namespace is {Length: 1};
    
    public async Task GetAdditionalParams() => _namespace = await this.GetNamespaces(isMultiselect: false, dialogService, viewModelFactory);
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllPagesWithPrefix(userPreferencesService.GetCurrentPref().UrlApi(), Param, _namespace!.First(), limit);
    }
}
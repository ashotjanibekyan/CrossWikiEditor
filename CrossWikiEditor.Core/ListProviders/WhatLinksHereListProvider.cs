using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class WhatLinksHereListProvider(
    IUserPreferencesService userPreferencesService,
    IViewModelFactory viewModelFactory,
    IDialogService dialogService,
    IPageService pageService) : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private NamespacesAndRedirectFilterOptions? _options;
    public override string Title => "What links here";
    public override string ParamTitle => "What links to";
    public override bool CanMake => !string.IsNullOrWhiteSpace(Param) && _options is not null;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetPagesLinkedTo(
            userPreferencesService.GetCurrentPref().UrlApi(),
            Param,
            _options!.Namespaces,
            filterRedirects: _options.RedirectFilter switch
            {
                RedirectFilter.All => null,
                RedirectFilter.Redirects => true,
                RedirectFilter.NoRedirects => false,
                _ => null
            },
            allowRedirectLinks: _options.IncludeRedirects,
            limit: limit);
    }

    public async Task GetAdditionalParams()
    {
        NamespacesAndRedirectFilterOptions? result =
            await dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(await viewModelFactory.GetSelectNamespacesAndRedirectFilterViewModel());
        if (result is not null)
        {
            _options = result;
        }
    }
}
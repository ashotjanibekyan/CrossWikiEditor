using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class WhatLinksHereListProvider(
    IUserPreferencesService userPreferencesService,
    IViewModelFactory viewModelFactory,
    IDialogService dialogService,
    IPageService pageService) : INeedAdditionalParamsListProvider
{
    private WhatLinksHereOptions? _options;
    public string Title => "What links here";
    public string ParamTitle => "What links to";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param) && _options is not null;

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await pageService.GetPagesLinkedTo(
            apiRoot: userPreferencesService.GetCurrentPref().UrlApi(),
            title: Param,
            namespaces: _options!.Namespaces,
            filterRedirects: _options.RedirectFilter switch
            {
                RedirectFilter.All => null,
                RedirectFilter.Redirects => true,
                RedirectFilter.NoRedirects => false,
                _ => null
            },
            allowRedirectLinks: _options.IncludeRedirects);
    }

    public async Task GetAdditionalParams()
    {
        WhatLinksHereOptions? result =
            await dialogService.ShowDialog<WhatLinksHereOptions>(await viewModelFactory.GetWhatLinksHereOptionsViewModel());
        if (result is not null)
        {
            _options = result;
        }
    }
}
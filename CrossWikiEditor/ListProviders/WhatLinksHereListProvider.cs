using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

public class WhatLinksHereListProvider(
    IUserPreferencesService userPreferencesService,
    IViewModelFactory viewModelFactory,
    IDialogService dialogService,
    IPageService pageService) : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private WhatLinksHereOptions? _options;
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
        WhatLinksHereOptions? result =
            await dialogService.ShowDialog<WhatLinksHereOptions>(await viewModelFactory.GetWhatLinksHereOptionsViewModel());
        if (result is not null)
        {
            _options = result;
        }
    }
}
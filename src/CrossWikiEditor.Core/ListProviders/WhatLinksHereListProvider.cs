using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class WhatLinksHereListProvider : LimitedListProviderBase, INeedAdditionalParamsListProvider
{
    private NamespacesAndRedirectFilterOptions? _options;
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;
    private readonly IViewModelFactory _viewModelFactory;

    public WhatLinksHereListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService,
        IViewModelFactory viewModelFactory) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
        _viewModelFactory = viewModelFactory;
    }

    public override string Title => "What links here";
    public override string ParamTitle => "What links to";
    public override bool CanMake => !string.IsNullOrWhiteSpace(Param) && _options is not null;

    public async Task GetAdditionalParams()
    {
        NamespacesAndRedirectFilterOptions? result =
            await DialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(await _viewModelFactory
                .GetSelectNamespacesAndRedirectFilterViewModel());
        if (result is not null)
        {
            _options = result;
        }
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.GetPagesLinkedTo(
            _settingsService.CurrentApiUrl,
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
}
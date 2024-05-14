namespace CrossWikiEditor.Core.ListProviders;

public sealed class WhatLinksHereListProvider(IDialogService dialogService,
    IPageService pageService, ISettingsService settingsService,
    IViewModelFactory viewModelFactory) : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private NamespacesAndRedirectFilterOptions? _options;
    public override string Title => "What links here";
    public override string ParamTitle => "What links to";
    public override bool CanMake => !string.IsNullOrWhiteSpace(Param) && _options is not null;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetPagesLinkedTo(
            settingsService.CurrentApiUrl,
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
            await DialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(await viewModelFactory.GetSelectNamespacesAndRedirectFilterViewModel());
        if (result is not null)
        {
            _options = result;
        }
    }
}
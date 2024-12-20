﻿namespace CrossWikiEditor.Core.ListProviders;

public sealed class RandomListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService,
    IViewModelFactory viewModelFactory)
    : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private NamespacesAndRedirectFilterOptions? _options;
    public override string Title => "Random pages";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _options is not null;

    public async Task GetAdditionalParams()
    {
        NamespacesAndRedirectFilterOptions? result =
            await DialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(
                await viewModelFactory.GetSelectNamespacesAndRedirectFilterViewModel(false));
        if (result is not null)
        {
            _options = result;
        }
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetRandomPages(
            settingsService.CurrentApiUrl,
            _options!.Namespaces,
            _options.RedirectFilter switch
            {
                RedirectFilter.All => null,
                RedirectFilter.Redirects => true,
                RedirectFilter.NoRedirects => false,
                _ => null
            },
            limit);
    }
}
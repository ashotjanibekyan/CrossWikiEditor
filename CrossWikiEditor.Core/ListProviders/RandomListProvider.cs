using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class RandomListProvider(IPageService pageService,
        IDialogService dialogService,
        IViewModelFactory viewModelFactory,
        IUserPreferencesService userPreferencesService)
    : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private NamespacesAndRedirectFilterOptions? _options;
    public override string Title => "Random pages";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _options is not null;

    public async Task GetAdditionalParams()
    {
        NamespacesAndRedirectFilterOptions? result =
            await dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(
                await viewModelFactory.GetSelectNamespacesAndRedirectFilterViewModel(isIncludeRedirectsVisible: false));
        if (result is not null)
        {
            _options = result;
        }
    }
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetRandomPages(
            userPreferencesService.CurrentApiUrl,
            _options!.Namespaces,
            filterRedirects: _options.RedirectFilter switch
            {
                RedirectFilter.All => null,
                RedirectFilter.Redirects => true,
                RedirectFilter.NoRedirects => false,
                _ => null
            },
            limit: limit);
    }
}
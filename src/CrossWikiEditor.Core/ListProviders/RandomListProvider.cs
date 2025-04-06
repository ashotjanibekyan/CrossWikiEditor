using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class RandomListProvider : LimitedListProviderBase, INeedAdditionalParamsListProvider
{
    private NamespacesAndRedirectFilterOptions? _options;
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;
    private readonly IViewModelFactory _viewModelFactory;

    public RandomListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService,
        IViewModelFactory viewModelFactory) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
        _viewModelFactory = viewModelFactory;
    }

    public override string Title => "Random pages";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _options is not null;

    public async Task GetAdditionalParams()
    {
        NamespacesAndRedirectFilterOptions? result =
            await DialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(
                await _viewModelFactory.GetSelectNamespacesAndRedirectFilterViewModel(false));
        if (result is not null)
        {
            _options = result;
        }
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.GetRandomPages(
            _settingsService.CurrentApiUrl,
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
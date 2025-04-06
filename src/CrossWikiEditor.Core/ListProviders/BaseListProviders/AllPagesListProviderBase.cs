using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Core.ListProviders.BaseListProviders;

public abstract class AllPagesListProviderBase : LimitedListProviderBase, INeedNamespacesListProvider
{
    private int[]? _namespaces;
    private readonly IPageService _pageService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ISettingsService _settingsService;

    protected AllPagesListProviderBase(IDialogService dialogService,
        IPageService pageService,
        IViewModelFactory viewModelFactory,
        ISettingsService settingsService) : base(dialogService)
    {
        _pageService = pageService;
        _viewModelFactory = viewModelFactory;
        _settingsService = settingsService;
    }

    public override string ParamTitle => "Start from";
    public override bool CanMake => _namespaces is {Length: 1};

    public async Task GetAdditionalParams()
    {
        _namespaces = await this.GetNamespaces(false, DialogService, _viewModelFactory);
    }

    protected async Task<Result<List<WikiPageModel>>> MakeListBase(int limit, PropertyFilterOption redirectsFilter,
        PropertyFilterOption langLinksFilter)
    {
        return await _pageService.GetAllPages(_settingsService.CurrentApiUrl, Param, _namespaces![0],
            redirectsFilter, langLinksFilter, limit);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class RecentChangesListProvider : LimitedListProviderBase, INeedNamespacesListProvider
{
    private int[]? _namespaces;
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;
    private readonly IViewModelFactory _viewModelFactory;

    public RecentChangesListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService,
        IViewModelFactory viewModelFactory) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
        _viewModelFactory = viewModelFactory;
    }

    public override string Title => "Recent Changes";
    public override string ParamTitle => "";
    public override bool CanMake => _namespaces is {Length: > 0};

    public async Task GetAdditionalParams()
    {
        _namespaces = await this.GetNamespaces(true, DialogService, _viewModelFactory);
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.GetRecentlyChangedPages(_settingsService.CurrentApiUrl, _namespaces, limit);
    }
}
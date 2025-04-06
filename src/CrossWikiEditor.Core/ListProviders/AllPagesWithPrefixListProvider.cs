using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllPagesWithPrefixListProvider : LimitedListProviderBase, INeedNamespacesListProvider
{
    private int[]? _namespaces;
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;
    private readonly IViewModelFactory _viewModelFactory;

    public AllPagesWithPrefixListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService,
        IViewModelFactory viewModelFactory) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
        _viewModelFactory = viewModelFactory;
    }

    public override string Title => "All Pages with prefix (Prefixindex)";
    public override string ParamTitle => "Prefix";
    public override bool CanMake => _namespaces is {Length: 1};

    public async Task GetAdditionalParams()
    {
        _namespaces = await this.GetNamespaces(false, DialogService, _viewModelFactory);
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.GetAllPagesWithPrefix(_settingsService.CurrentApiUrl, Param, _namespaces![0], limit);
    }
}
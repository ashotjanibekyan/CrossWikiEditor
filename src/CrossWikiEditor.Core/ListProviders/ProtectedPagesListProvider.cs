using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class ProtectedPagesListProvider : LimitedListProviderBase, INeedAdditionalParamsListProvider
{
    private string _protectionLevel = string.Empty;
    private string _protectionType = string.Empty;
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;
    private readonly IViewModelFactory _viewModelFactory;

    public ProtectedPagesListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService,
        IViewModelFactory viewModelFactory) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
        _viewModelFactory = viewModelFactory;
    }

    public override string Title => "Protected pages";
    public override string ParamTitle => "";
    public override bool CanMake => !string.IsNullOrWhiteSpace(_protectionType) && !string.IsNullOrWhiteSpace(_protectionLevel);

    public async Task GetAdditionalParams()
    {
        (_protectionType, _protectionLevel) =
            await DialogService.ShowDialog<(string, string)>(_viewModelFactory.GetSelectProtectionSelectionPageViewModel());
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.GetProtectedPages(_settingsService.CurrentApiUrl, _protectionType, _protectionLevel, limit);
    }
}
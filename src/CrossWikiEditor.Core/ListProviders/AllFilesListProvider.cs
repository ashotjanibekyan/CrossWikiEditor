using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllFilesListProvider : LimitedListProviderBase
{
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;

    public AllFilesListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
    }

    public override string Title => "All Files";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.GetAllFiles(_settingsService.CurrentApiUrl, Param, limit);
    }
}
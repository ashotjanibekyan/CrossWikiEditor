using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class ImageFileLinksListProvider : LimitedListProviderBase
{
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;

    public ImageFileLinksListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
    }

    public override string Title => "Image file links";
    public override string ParamTitle => "File";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.GetPagesByFileUsage(_settingsService.CurrentApiUrl, Param, limit);
    }
}
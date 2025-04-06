using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class WikiSearchInTitleAllNsListProvider : LimitedListProviderBase
{
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;

    public WikiSearchInTitleAllNsListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
    }

    public override string Title => "Wiki search (title) (all NS)";
    public override string ParamTitle => "Wiki search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.WikiSearch(_settingsService.CurrentApiUrl, $"intitle:{Param}", [], limit);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class WhatTranscludesHereAllNsListProvider : LimitedListProviderBase
{
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;

    public WhatTranscludesHereAllNsListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService) : base(dialogService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
    }

    public override string Title => "What transcludes page (all NS)";
    public override string ParamTitle => "What embeds";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _pageService.GetTransclusionsOf(_settingsService.CurrentApiUrl, Param, null, limit);
    }
}
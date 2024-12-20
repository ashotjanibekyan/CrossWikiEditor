﻿namespace CrossWikiEditor.Core.ListProviders;

public sealed class WikiSearchInTextAllNsListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Wiki search (text) (all NS)";
    public override string ParamTitle => "Wiki search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.WikiSearch(settingsService.CurrentApiUrl, Param, [], limit);
    }
}
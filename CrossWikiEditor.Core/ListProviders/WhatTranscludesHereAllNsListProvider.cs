﻿namespace CrossWikiEditor.Core.ListProviders;

public sealed class WhatTranscludesHereAllNsListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "What transcludes page (all NS)";
    public override string ParamTitle => "What embeds";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetTransclusionsOf(userPreferencesService.CurrentApiUrl, Param, null, limit);
}
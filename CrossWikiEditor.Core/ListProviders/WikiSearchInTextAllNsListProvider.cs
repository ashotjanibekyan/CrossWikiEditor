﻿namespace CrossWikiEditor.Core.ListProviders;

public sealed class WikiSearchInTextAllNsListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Wiki search (text) (all NS)";
    public override string ParamTitle => "Wiki search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.WikiSearch(userPreferencesService.CurrentApiUrl, Param, null, limit);
}
namespace CrossWikiEditor.Core.ListProviders;

public sealed class WhatTranscludesHereListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "What transcludes page";
    public override string ParamTitle => "What embeds";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetTransclusionsOf(userPreferencesService.CurrentApiUrl, Param, new[] {0}, limit);
}
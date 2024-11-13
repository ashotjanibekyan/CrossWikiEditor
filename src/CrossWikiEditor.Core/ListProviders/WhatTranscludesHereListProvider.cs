namespace CrossWikiEditor.Core.ListProviders;

public sealed class WhatTranscludesHereListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "What transcludes page";
    public override string ParamTitle => "What embeds";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetTransclusionsOf(settingsService.CurrentApiUrl, Param, [0], limit);
    }
}
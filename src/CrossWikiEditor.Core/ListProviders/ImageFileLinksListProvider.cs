﻿namespace CrossWikiEditor.Core.ListProviders;

public sealed class ImageFileLinksListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Image file links";
    public override string ParamTitle => "File";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetPagesByFileUsage(settingsService.CurrentApiUrl, Param, limit);
    }
}
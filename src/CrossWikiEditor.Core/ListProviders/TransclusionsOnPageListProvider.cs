namespace CrossWikiEditor.Core.ListProviders;

public sealed class TransclusionsOnPageListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Transclusions on page";
    public override string ParamTitle => "Transclusions on";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetTransclusionsOn(settingsService.CurrentApiUrl, Param, limit);
    }
}
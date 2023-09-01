using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

public sealed class TransclusionsOnPageListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Transclusions on page";
    public override string ParamTitle => "Transclusions on";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetTransclusionsOn(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
}
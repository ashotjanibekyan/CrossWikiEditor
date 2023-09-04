using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class LinksOnPageListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Links on page";
    public override string ParamTitle => "Links on";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.LinksOnPage(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
}
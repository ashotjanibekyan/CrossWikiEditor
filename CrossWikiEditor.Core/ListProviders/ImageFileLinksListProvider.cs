using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class ImageFileLinksListProvider(
    IPageService pageService,
    IUserPreferencesService userPreferencesService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Image file links";
    public override string ParamTitle => "File";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetPagesByFileUsage(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
}
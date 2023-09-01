using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

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
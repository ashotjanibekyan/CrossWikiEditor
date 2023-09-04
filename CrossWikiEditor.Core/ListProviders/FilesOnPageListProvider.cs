using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class FilesOnPageListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Files on page";
    public override string ParamTitle => "Files on";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        return await pageService.FilesOnPage(userPrefs.UrlApi(), Param, limit);
    }
}
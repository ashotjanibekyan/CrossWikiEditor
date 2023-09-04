using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class CategoriesOnPageNoHiddenCategoriesListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService,
        IDialogService dialogService) 
    : CategoriesOnPageListProvider(userPreferencesService, pageService, dialogService)
{
    public override string Title => "Categories on page (no hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        return await pageService.GetCategoriesOf(userPrefs.UrlApi(), Param, limit, false);
    }
}
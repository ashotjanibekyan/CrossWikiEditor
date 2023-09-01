using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Settings;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

public class CategoriesOnPageOnlyHiddenCategoriesListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService,
        IDialogService dialogService)
    : CategoriesOnPageListProvider(userPreferencesService, pageService, dialogService)
{
    public override string Title => "Categories on page (only hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        return await pageService.GetCategoriesOf(userPrefs.UrlApi(), Param, limit, onlyHidden: true);
    }
}
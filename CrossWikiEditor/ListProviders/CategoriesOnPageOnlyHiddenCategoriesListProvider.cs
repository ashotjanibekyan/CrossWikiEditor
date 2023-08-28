using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Settings;

namespace CrossWikiEditor.ListProviders;

public class CategoriesOnPageOnlyHiddenCategoriesListProvider(IPageService pageService, IUserPreferencesService userPreferencesService)
    : CategoriesOnPageListProvider(pageService, userPreferencesService)
{
    public override string Title => "Categories on page (only hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        return await pageService.GetCategoriesOf(userPrefs.UrlApi(), Param, onlyHidden: true);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class CategoriesOnPageNoHiddenCategoriesListProvider : CategoriesOnPageListProvider
{
    public CategoriesOnPageNoHiddenCategoriesListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService) : base(pageService, userPreferencesService)
    {
    }

    public override string Title => "Categories on page (no hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
        return await _pageService.GetCategoriesOf(userPrefs.UrlApi(), Param, includeHidden: false);
    }
}
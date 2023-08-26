using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor.ListProviders;

public class CategoriesOnPageOnlyHiddenCategoriesListProvider : CategoriesOnPageListProvider
{
    public CategoriesOnPageOnlyHiddenCategoriesListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService) : base(pageService, userPreferencesService)
    {
    }

    public override string Title => "Categories on page (only hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
        return await _pageService.GetCategoriesOf(userPrefs.UrlApi(), Param, onlyHidden: true);
    }
}
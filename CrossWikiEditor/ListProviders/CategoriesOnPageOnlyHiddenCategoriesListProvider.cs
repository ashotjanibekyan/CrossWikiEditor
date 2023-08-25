using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class CategoriesOnPageOnlyHiddenCategoriesListProvider : CategoriesOnPageListProvider
{
    public CategoriesOnPageOnlyHiddenCategoriesListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService) : base(pageService, userPreferencesService)
    {
    }

    public override string Title => "Categories on page (only hidden categories)";

    public override async Task<Result<List<string>>> MakeList()
    {
        UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
        return await _pageService.GetCategoriesOf(userPrefs.ApiRoot(), Param, onlyHidden: true);
    }
}
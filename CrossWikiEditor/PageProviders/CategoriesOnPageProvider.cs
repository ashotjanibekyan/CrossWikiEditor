using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.PageProviders;

public class CategoriesOnPageProvider : IPageProvider
{
    private readonly IPageService _pageService;
    private readonly IUserPreferencesService _userPreferencesService;

    public CategoriesOnPageProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService)
    {
        _pageService = pageService;
        _userPreferencesService = userPreferencesService;
    }
    public string Title => "Categories on page";
    public string ParamTitle => "Page";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public async Task<Result<List<string>>> MakeList()
    {
        UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
        return await _pageService.GetCategoriesOf(userPrefs.Site, Param);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor.ListProviders;

public class CategoriesOnPageListProvider : IListProvider
{
    protected readonly IPageService _pageService;
    protected readonly IUserPreferencesService _userPreferencesService;

    public CategoriesOnPageListProvider(
        IPageService pageService,
        IUserPreferencesService userPreferencesService)
    {
        _pageService = pageService;
        _userPreferencesService = userPreferencesService;
    }
    public virtual string Title => "Categories on page";
    public string ParamTitle => "Page";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public bool NeedsAdditionalParams => false;

    public virtual async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
        return await _pageService.GetCategoriesOf(userPrefs.UrlApi(), Param);
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }
}
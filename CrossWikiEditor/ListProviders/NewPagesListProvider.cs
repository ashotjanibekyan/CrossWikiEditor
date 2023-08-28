using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class NewPagesListProvider : IListProvider
{
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IPageService _pageService;

    public NewPagesListProvider(IUserPreferencesService userPreferencesService, IPageService pageService)
    {
        _userPreferencesService = userPreferencesService;
        _pageService = pageService;
    }
    public string Title => "New pages";
    public string ParamTitle => string.Empty;
    public string Param { get; set; } = string.Empty;
    public bool CanMake => true;
    public bool NeedsAdditionalParams => false;
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await _pageService.GetNewPages(_userPreferencesService.GetCurrentPref().UrlApi());
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }
}
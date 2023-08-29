using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class WikiSearchInTextListProvider(IUserPreferencesService userPreferencesService, IPageService pageService) : IListProvider
{
    public string Title => "Wiki search (text)";
    public string ParamTitle => "Wiki search";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        string apiRoot = userPreferencesService.GetCurrentPref().UrlApi();
        return await pageService.WikiSearch(apiRoot, Param, new []{0});
    }
}
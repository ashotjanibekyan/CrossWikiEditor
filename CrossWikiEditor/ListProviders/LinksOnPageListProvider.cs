using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class LinksOnPageListProvider(IUserPreferencesService userPreferencesService, IPageService pageService) : IListProvider
{
    public virtual string Title => "Links on page";
    public string ParamTitle => "Links on";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public bool NeedsAdditionalParams => false;

    public virtual async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await pageService.LinksOnPage(userPreferencesService.GetCurrentPref().UrlApi(), Param);
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }
}
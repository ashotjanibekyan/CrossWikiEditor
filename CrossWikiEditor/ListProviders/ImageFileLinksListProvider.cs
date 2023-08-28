using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class ImageFileLinksListProvider(IPageService pageService,
        IUserPreferencesService userPreferencesService)
    : IListProvider
{
    public string Title => "Image file links";
    public string ParamTitle => "File";
    public string Param { get; set; }
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public bool NeedsAdditionalParams => false;

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await pageService.GetPagesByFileUsage(userPreferencesService.GetCurrentPref().UrlApi(), Param);
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }
}
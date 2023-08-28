using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class FilesOnPageListProvider(IPageService pageService,
        IUserPreferencesService userPreferencesService)
    : IListProvider
{
    public string Title => "Files on page";
    public string ParamTitle => "Files on";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public bool NeedsAdditionalParams => false;

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        return await pageService.FilesOnPage(userPrefs.UrlApi(), Param);
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }
}
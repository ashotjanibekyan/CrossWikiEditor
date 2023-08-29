using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Settings;

namespace CrossWikiEditor.ListProviders;

public class CategoryRecursive1LevelListProvider(IPageService pageService,
        IUserPreferencesService userPreferencesService)
    : IListProvider
{
    public string Title => "Category (recursive 1 level)";
    public string ParamTitle => "Category";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        return await pageService.GetPagesOfCategory(userPrefs.UrlApi(), Param, 1);
    }
}
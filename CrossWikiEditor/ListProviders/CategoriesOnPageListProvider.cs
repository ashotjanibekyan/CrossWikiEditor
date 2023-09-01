using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Settings;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

public class CategoriesOnPageListProvider(IPageService pageService, IUserPreferencesService userPreferencesService) : IListProvider
{
    public virtual string Title => "Categories on page";
    public string ParamTitle => "Page";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);

    public virtual async Task<Result<List<WikiPageModel>>> MakeList()
    {
        UserPrefs userPrefs = userPreferencesService.GetCurrentPref();
        return await pageService.GetCategoriesOf(userPrefs.UrlApi(), Param);
    }
}
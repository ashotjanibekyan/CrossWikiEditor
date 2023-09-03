using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders.SpecialPageListProviders;

public class AllCategoriesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), ISpecialPageListProvider
{
    public override string Title => "All Categories";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;
    public int NamespaceId { get; set; }
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        
        return await pageService.GetAllCategories(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
}
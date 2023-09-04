using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class AllCategoriesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "All Categories";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllCategories(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
}
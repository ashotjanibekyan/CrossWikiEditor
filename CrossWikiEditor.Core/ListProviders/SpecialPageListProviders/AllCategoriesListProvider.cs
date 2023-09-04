using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;

public class AllCategoriesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), ISpecialPageListProvider
{
    public override string Title => "All Categories";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;
    public int NamespaceId { get; set; }
    public bool NeedsNamespace => false;
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllCategories(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
}
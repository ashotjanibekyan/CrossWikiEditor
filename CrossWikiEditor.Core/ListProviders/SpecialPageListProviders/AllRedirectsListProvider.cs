using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;

public class AllRedirectsListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), ISpecialPageListProvider
{
    public override string Title => "All Redirects";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;
    public int NamespaceId { get; set; }
    public bool NeedsNamespace => true;
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllPages(userPreferencesService.GetCurrentPref().UrlApi(), Param, NamespaceId,
            redirectsFilter: PropertyFilterOption.WithProperty, limit);
    }
}
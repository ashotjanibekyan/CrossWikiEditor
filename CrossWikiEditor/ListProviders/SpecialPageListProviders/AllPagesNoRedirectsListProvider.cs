using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.ListProviders.SpecialPageListProviders;

public class AllPagesNoRedirectsListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), ISpecialPageListProvider
{
    public override string Title => "All Pages (no redirects)";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;
    public int NamespaceId { get; set; }
    public bool NeedsNamespace => true;
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllPages(userPreferencesService.GetCurrentPref().UrlApi(), Param, NamespaceId,
            redirectsFilter: PropertyFilterOption.WithoutProperty, limit);
    }
}
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;

public class AllPagesWithPrefixListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), ISpecialPageListProvider
{
    public override string Title => "All Pages with prefix (Prefixindex)";
    public override string ParamTitle => "Prefix";
    public override bool CanMake => true;
    public int NamespaceId { get; set; }
    public bool NeedsNamespace => true;
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllPagesWithPrefix(userPreferencesService.GetCurrentPref().UrlApi(), Param, NamespaceId, limit);
    }
}
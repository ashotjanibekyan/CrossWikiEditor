using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class PagesWithPropListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService), ISpecialPageListProvider
{
    public override string Title => "Pages with a page property";
    public override string ParamTitle => "Property name";
    public int NamespaceId { get; set; }
    public bool NeedsNamespace => false;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        string apiRoot = userPreferencesService.GetCurrentPref().UrlApi();
        return await pageService.GetPagesWithProp(apiRoot, Param, limit);
    }
}
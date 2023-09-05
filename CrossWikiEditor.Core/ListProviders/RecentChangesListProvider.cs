using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class RecentChangesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService,
    IViewModelFactory viewModelFactory) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string Title => "Recent Changes";
    public override string ParamTitle => "";
    public override bool CanMake => _namespaces is {Length: > 0};
    
    public async Task GetAdditionalParams()
    {
        _namespaces = await this.GetNamespaces(true, dialogService, viewModelFactory);
    }
    
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        string apiRoot = userPreferencesService.GetCurrentPref().UrlApi();
        return await pageService.GetRecentlyChangedPages(apiRoot, _namespaces, limit);
    }
}
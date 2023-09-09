using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllPagesWithPrefixListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IViewModelFactory viewModelFactory,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string Title => "All Pages with prefix (Prefixindex)";
    public override string ParamTitle => "Prefix";
    public override bool CanMake => _namespaces is {Length: 1};
    
    public async Task GetAdditionalParams() => _namespaces = await this.GetNamespaces(isMultiselect: false, dialogService, viewModelFactory);
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllPagesWithPrefix(userPreferencesService.GetCurrentPref().UrlApi(), Param, _namespaces!.First(), limit);
    }
}
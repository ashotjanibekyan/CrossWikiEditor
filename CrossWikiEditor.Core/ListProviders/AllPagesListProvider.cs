using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllPagesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IViewModelFactory viewModelFactory,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespace;
    public override string Title => "All Pages";
    public override string ParamTitle => "Start from";
    public override bool CanMake => _namespace is {Length: 1};
    
    public async Task GetAdditionalParams() => _namespace = await this.GetNamespaces(isMultiselect: false, dialogService, viewModelFactory);
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllPages(userPreferencesService.GetCurrentPref().UrlApi(), Param, _namespace!.First(),
            redirectsFilter: PropertyFilterOption.Disable, langLinksFilter: PropertyFilterOption.Disable, limit);
    }
}
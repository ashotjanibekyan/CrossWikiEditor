using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Core.ListProviders;

public class PagesWithoutLanguageLinksListProvider(
    IDialogService dialogService,
    IViewModelFactory viewModelFactory,
    IUserPreferencesService userPreferencesService,
    IPageService pageService) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    protected int[]? _namespaces;
    public override string Title => "Pages without language links";
    public override string ParamTitle => "Start from";
    public override bool CanMake => _namespaces is {Length: 1};
    public async Task GetAdditionalParams() => _namespaces = await this.GetNamespaces(false, dialogService, viewModelFactory);

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        string apiRoot = userPreferencesService.GetCurrentPref().UrlApi();
        return await pageService.GetAllPages(apiRoot, Param, _namespaces!.First(), PropertyFilterOption.Disable, PropertyFilterOption.WithoutProperty, limit);
    }
}
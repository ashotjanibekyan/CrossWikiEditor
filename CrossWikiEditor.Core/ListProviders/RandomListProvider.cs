using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class RandomListProvider(IPageService pageService,
        IDialogService dialogService,
        IViewModelFactory viewModelFactory,
        IUserPreferencesService userPreferencesService)
    : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string Title => "Random pages";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _namespaces is not null;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        Result<List<WikiPageModel>> result = await pageService.GetRandomPages(userPreferencesService.GetCurrentPref().UrlApi(), _namespaces, limit);
        _namespaces = null;
        return result;
    }

    public async Task GetAdditionalParams() => _namespaces = await this.GetNamespaces(isMultiselect: true, dialogService, viewModelFactory);
}
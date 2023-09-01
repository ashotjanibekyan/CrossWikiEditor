using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

public class RandomListProvider(IPageService pageService,
        IDialogService dialogService,
        IViewModelFactory viewModelFactory,
        IUserPreferencesService userPreferencesService)
    : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
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

    public async Task GetAdditionalParams()
    {
        int[]? result = await dialogService.ShowDialog<int[]>(await viewModelFactory.GetSelectNamespacesViewModel());
        if (result is not null)
        {
            _namespaces = result;
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class RandomListProvider : IListProvider
{
    private readonly IPageService _pageService;
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IUserPreferencesService _userPreferencesService;
    private int[]? _namespaces;

    public RandomListProvider(
        IPageService pageService,
        IDialogService dialogService,
        IViewModelFactory viewModelFactory,
        IUserPreferencesService userPreferencesService)
    {
        _pageService = pageService;
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;
        _userPreferencesService = userPreferencesService;
    }
    public string Title => "Random pages";
    public string ParamTitle => string.Empty;
    public string Param { get; set; } = string.Empty;
    public bool CanMake => _namespaces is not null;
    public bool NeedsAdditionalParams => true;
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        var result = await _pageService.GetRandomPages(_userPreferencesService.GetCurrentPref().UrlApi(), 500, _namespaces);
        _namespaces = null;
        return result;

    }

    public async Task GetAdditionalParams()
    {
        var result = await _dialogService.ShowDialog<Result<int[]>>(await _viewModelFactory.GetSelectNamespacesViewModel());
        if (result is {IsSuccessful: true, Value: not null})
        {
            _namespaces = result.Value;
        }
    }
}
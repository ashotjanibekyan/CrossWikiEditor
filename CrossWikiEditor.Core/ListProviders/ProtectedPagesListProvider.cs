using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class ProtectedPagesListProvider(
    IUserPreferencesService userPreferencesService, 
    IViewModelFactory viewModelFactory,
    IDialogService dialogService,
    IPageService pageService) : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private string _protectionType = string.Empty;
    private string _protectionLevel = string.Empty;
    public override string Title => "Protected pages";
    public override string ParamTitle => "";
    public override bool CanMake => !string.IsNullOrWhiteSpace(_protectionType) && !string.IsNullOrWhiteSpace(_protectionLevel);

    public async Task GetAdditionalParams()
    {
        (_protectionType, _protectionLevel) = await dialogService.ShowDialog<(string, string)>(viewModelFactory.GetSelectProtectionSelectionPageViewModel());
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        string apiRoot = userPreferencesService.GetCurrentPref().UrlApi();
        return await pageService.GetProtectedPages(apiRoot, _protectionType, _protectionLevel, limit);
    }
}
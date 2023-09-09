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

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetProtectedPages(userPreferencesService.CurrentApiUrl, _protectionType, _protectionLevel, limit);
}
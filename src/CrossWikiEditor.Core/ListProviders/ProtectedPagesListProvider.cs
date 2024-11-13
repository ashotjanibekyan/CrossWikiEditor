namespace CrossWikiEditor.Core.ListProviders;

public sealed class ProtectedPagesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    ISettingsService settingsService,
    IViewModelFactory viewModelFactory) : LimitedListProviderBase(dialogService), INeedAdditionalParamsListProvider
{
    private string _protectionLevel = string.Empty;
    private string _protectionType = string.Empty;
    public override string Title => "Protected pages";
    public override string ParamTitle => "";
    public override bool CanMake => !string.IsNullOrWhiteSpace(_protectionType) && !string.IsNullOrWhiteSpace(_protectionLevel);

    public async Task GetAdditionalParams()
    {
        (_protectionType, _protectionLevel) =
            await DialogService.ShowDialog<(string, string)>(viewModelFactory.GetSelectProtectionSelectionPageViewModel());
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetProtectedPages(settingsService.CurrentApiUrl, _protectionType, _protectionLevel, limit);
    }
}
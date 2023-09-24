namespace CrossWikiEditor.Core.ListProviders;

public sealed class DatabaseDumpListProvider(IDialogService dialogService, IViewModelFactory viewModelFactory) : UnlimitedListProviderBase
{
    public override string Title => "Database dump";
    public override string ParamTitle => "";
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        List<WikiPageModel> result = await dialogService.ShowDialog<List<WikiPageModel>?>(viewModelFactory.GetDatabaseScannerViewModel()) ??
                                     new List<WikiPageModel>();
        return Result<List<WikiPageModel>>.Success(result);
    }
}
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class DatabaseDumpListProvider(IDialogService dialogService, IViewModelFactory viewModelFactory) : UnlimitedListProviderBase
{
    public override string Title => "Database dump";
    public override string ParamTitle => "";
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        List<WikiPageModel> result = await dialogService.ShowDialog<List<WikiPageModel>?>(await viewModelFactory.GetDatabaseScannerViewModel()) ?? new List<WikiPageModel>();
        return Result<List<WikiPageModel>>.Success(result);
    }
}
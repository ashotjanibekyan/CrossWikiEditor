using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class DatabaseDumpListProvider : UnlimitedListProviderBase
{
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;

    public DatabaseDumpListProvider(IDialogService dialogService, IViewModelFactory viewModelFactory)
    {
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;
    }

    public override string Title => "Database dump";
    public override string ParamTitle => "";
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await _dialogService.ShowDialog<List<WikiPageModel>?>(_viewModelFactory.GetDatabaseScannerViewModel()) ?? [];
    }
}
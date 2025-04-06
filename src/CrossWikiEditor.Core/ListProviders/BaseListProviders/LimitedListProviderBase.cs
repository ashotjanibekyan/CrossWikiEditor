using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.ViewModels;

namespace CrossWikiEditor.Core.ListProviders.BaseListProviders;

public interface ILimitedListProvider : IListProvider
{
    Task<int> GetLimit();
    Task<Result<List<WikiPageModel>>> MakeList(int limit);
}

public abstract class LimitedListProviderBase : ListProviderBase, ILimitedListProvider
{
    private readonly IDialogService _dialogService;

    protected LimitedListProviderBase(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    protected IDialogService DialogService => _dialogService;

    public async Task<int> GetLimit()
    {
        return await _dialogService.ShowDialog<int?>(new PromptViewModel("How many page", "Limit: ")
        {
            IsNumeric = true,
            Value = 50
        }) ?? 50;
    }

    public abstract Task<Result<List<WikiPageModel>>> MakeList(int limit);
}
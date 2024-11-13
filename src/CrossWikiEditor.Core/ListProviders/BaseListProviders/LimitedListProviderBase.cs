namespace CrossWikiEditor.Core.ListProviders.BaseListProviders;

public interface ILimitedListProvider : IListProvider
{
    Task<int> GetLimit();
    Task<Result<List<WikiPageModel>>> MakeList(int limit);
}

public abstract class LimitedListProviderBase(IDialogService dialogService) : ListProviderBase, ILimitedListProvider
{
    protected IDialogService DialogService => dialogService;

    public async Task<int> GetLimit()
    {
        return await dialogService.ShowDialog<int?>(new PromptViewModel("How many page", "Limit: ")
        {
            IsNumeric = true,
            Value = 50
        }) ?? 50;
    }

    public abstract Task<Result<List<WikiPageModel>>> MakeList(int limit);
}
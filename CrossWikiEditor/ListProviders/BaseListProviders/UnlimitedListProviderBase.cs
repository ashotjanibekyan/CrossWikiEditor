using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders.BaseListProviders;

public interface IUnlimitedListProvider : IListProvider
{
    Task<Result<List<WikiPageModel>>> MakeList();
}

public abstract class UnlimitedListProviderBase : ListProviderBase, IUnlimitedListProvider
{
    public abstract Task<Result<List<WikiPageModel>>> MakeList();
}
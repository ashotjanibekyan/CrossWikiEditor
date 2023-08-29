using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;

namespace CrossWikiEditor.ListProviders;

public interface IListProvider
{
    string Title { get; }
    string ParamTitle { get; }
    string Param { get; set; }
    bool CanMake { get; }
    Task<Result<List<WikiPageModel>>> MakeList();
}
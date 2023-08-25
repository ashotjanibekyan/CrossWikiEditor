using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossWikiEditor.PageProviders;

public interface IPageProvider
{
    string Title { get; }
    string ParamTitle { get; }
    string Param { get; set; }
    bool CanMake { get; }
    Task<Result<List<string>>> MakeList();
}
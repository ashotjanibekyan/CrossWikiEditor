using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossWikiEditor.PageProviders;

public interface IListProvider
{
    string Title { get; }
    string ParamTitle { get; }
    string Param { get; set; }
    bool CanMake { get; }
    bool NeedsAdditionalParams { get; }
    Task<Result<List<string>>> MakeList();
    Task GetAdditionalParams();
}
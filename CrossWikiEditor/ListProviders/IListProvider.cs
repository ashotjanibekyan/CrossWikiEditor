using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor.ListProviders;

public interface IListProvider
{
    string Title { get; }
    string ParamTitle { get; }
    string Param { get; set; }
    bool CanMake { get; }
    bool NeedsAdditionalParams { get; }
    Task<Result<List<WikiPageModel>>> MakeList();
    Task GetAdditionalParams();
}
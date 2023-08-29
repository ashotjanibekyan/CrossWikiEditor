using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class MyWatchlistListProvider(IUserService userService) : IListProvider
{
    public string Title => "My watchlist";
    public string ParamTitle => string.Empty;
    public string Param { get; set; } = string.Empty;
    public bool CanMake => true;

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await userService.GetWatchlistPages();
    }
}
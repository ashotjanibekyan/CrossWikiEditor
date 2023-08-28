using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class MyWatchlistListProvider : IListProvider
{
    private readonly IUserService _userService;

    public MyWatchlistListProvider(IUserService userService)
    {
        _userService = userService;
    }
    public string Title => "My watchlist";
    public string ParamTitle => string.Empty;
    public string Param { get; set; } = string.Empty;
    public bool CanMake => true;
    public bool NeedsAdditionalParams => false;
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await _userService.GetWatchlistPages();
    }

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }
}
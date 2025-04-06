using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class MyWatchlistListProvider : LimitedListProviderBase
{
    private readonly IUserService _userService;

    public MyWatchlistListProvider(IDialogService dialogService, IUserService userService) : base(dialogService)
    {
        _userService = userService;
    }

    public override string Title => "My watchlist";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _userService.GetWatchlistPages(limit);
    }
}
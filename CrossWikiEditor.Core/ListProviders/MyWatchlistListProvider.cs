﻿using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class MyWatchlistListProvider(
    IUserService userService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "My watchlist";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await userService.GetWatchlistPages(limit);
    }
}
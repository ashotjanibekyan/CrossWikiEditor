namespace CrossWikiEditor.Core.ListProviders;

public sealed class MyWatchlistListProvider(
    IUserService userService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "My watchlist";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => true;

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) => await userService.GetWatchlistPages(limit);
}
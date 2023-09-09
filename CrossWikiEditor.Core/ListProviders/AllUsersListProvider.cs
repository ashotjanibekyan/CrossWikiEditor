namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllUsersListProvider(
    IDialogService dialogService,
    IUserService userService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "All Users";
    public override string ParamTitle => "Start from";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await userService.GetAllUsers(userPreferencesService.CurrentApiUrl, Param, limit);
}
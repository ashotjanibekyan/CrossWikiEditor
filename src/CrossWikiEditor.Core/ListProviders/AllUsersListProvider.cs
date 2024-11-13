namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllUsersListProvider(
    IDialogService dialogService,
    ISettingsService settingsService,
    IUserService userService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "All Users";
    public override string ParamTitle => "Start from";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await userService.GetAllUsers(settingsService.CurrentApiUrl, Param, limit);
    }
}
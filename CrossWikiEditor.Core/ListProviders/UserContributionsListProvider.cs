namespace CrossWikiEditor.Core.ListProviders;

public sealed class UserContributionsListProvider(
    IDialogService dialogService,
    IUserService userService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "User contribs";
    public override string ParamTitle => "User";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await userService.GetUserContributionsPages(userPreferencesService.CurrentApiUrl, Param, limit);
}
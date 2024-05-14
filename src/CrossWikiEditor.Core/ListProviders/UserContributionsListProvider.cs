namespace CrossWikiEditor.Core.ListProviders;

public sealed class UserContributionsListProvider(IDialogService dialogService,
    ISettingsService settingsService,
    IUserService userService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "User contribs";
    public override string ParamTitle => "User";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await userService.GetUserContributionsPages(settingsService.CurrentApiUrl, Param, limit);
}
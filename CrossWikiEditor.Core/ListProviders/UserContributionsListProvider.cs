using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class UserContributionsListProvider(
    IUserPreferencesService userPreferencesService,
    IUserService userService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "User contribs";
    public override string ParamTitle => "User";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await userService.GetUserContributionsPages(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
}
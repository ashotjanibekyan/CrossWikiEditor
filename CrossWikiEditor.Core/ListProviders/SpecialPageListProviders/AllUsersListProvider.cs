using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;

public class AllUsersListProvider(
    IDialogService dialogService,
    IUserService userService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), ISpecialPageListProvider
{
    public override string Title => "All Users";
    public override string ParamTitle => "Start from";
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await userService.GetAllUsers(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
    public int NamespaceId { get; set; }
    public bool NeedsNamespace => false;
}
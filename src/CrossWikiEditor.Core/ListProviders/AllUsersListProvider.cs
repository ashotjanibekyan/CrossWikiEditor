using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllUsersListProvider : LimitedListProviderBase
{
    private readonly ISettingsService _settingsService;
    private readonly IUserService _userService;

    public AllUsersListProvider(IDialogService dialogService,
        ISettingsService settingsService,
        IUserService userService) : base(dialogService)
    {
        _settingsService = settingsService;
        _userService = userService;
    }

    public override string Title => "All Users";
    public override string ParamTitle => "Start from";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _userService.GetAllUsers(_settingsService.CurrentApiUrl, Param, limit);
    }
}
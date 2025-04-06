using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class UserContributionsListProvider : LimitedListProviderBase
{
    private readonly ISettingsService _settingsService;
    private readonly IUserService _userService;

    public UserContributionsListProvider(IDialogService dialogService,
        ISettingsService settingsService,
        IUserService userService) : base(dialogService)
    {
        _settingsService = settingsService;
        _userService = userService;
    }

    public override string Title => "User contribs";
    public override string ParamTitle => "User";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _userService.GetUserContributionsPages(_settingsService.CurrentApiUrl, Param, limit);
    }
}
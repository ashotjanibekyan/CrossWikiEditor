using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;
public sealed class UserContribsListProvider(IUserPreferencesService userPreferencesService, IUserService userService) : IListProvider
{
    public string Title => "User contribs";
    public string ParamTitle => "User";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(ParamTitle);
    
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        return await userService.GetUserContribsPages(userPreferencesService.GetCurrentPref().UrlApi(), Param);
    }
}

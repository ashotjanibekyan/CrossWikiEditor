using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class LinksOnPageRedListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService) : LinksOnPageListProvider(userPreferencesService, pageService)
{
    public override string Title => "Links on page (only bluelinks)";

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        Result<List<WikiPageModel>> result = await base.MakeList();
        if (result is {IsSuccessful: true, Value: not null})
        {
            return Result<List<WikiPageModel>>.Success(result.Value.Where(x => x.WikiPage is not null && !x.WikiPage.Exists).ToList());
        }

        return result;
    }
}
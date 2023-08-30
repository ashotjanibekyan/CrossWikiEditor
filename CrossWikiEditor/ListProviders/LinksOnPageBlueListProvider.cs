using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.ListProviders;

public class LinksOnPageBlueListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService) : LinksOnPageListProvider(userPreferencesService, pageService)
{
    public override string Title => "Links on page (only bluelinks)";

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        Result<List<WikiPageModel>> result = await base.MakeList();
        if (result is not {IsSuccessful: true, Value: not null})
        {
            return result;
        }

        var existsTable = await Task.WhenAll(result.Value.Select(p => p.Exists()));
        return Result<List<WikiPageModel>>.Success(result.Value.Where((_, index) => existsTable[index]).ToList());
    }
}
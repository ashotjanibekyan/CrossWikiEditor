using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

public class LinksOnPageRedListProvider(
        IUserPreferencesService userPreferencesService,
        IPageService pageService,
        IDialogService dialogService)
    : LinksOnPageListProvider(userPreferencesService, pageService, dialogService)
{
    public override string Title => "Links on page (only bluelinks)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        Result<List<WikiPageModel>> result = await base.MakeList(limit);
        if (result is not {IsSuccessful: true, Value: not null})
        {
            return result;
        }

        bool[] existsTable = await Task.WhenAll(result.Value.Select(p => p.Exists()));
        return Result<List<WikiPageModel>>.Success(result.Value.Where((_, index) => !existsTable[index]).ToList());
    }
}
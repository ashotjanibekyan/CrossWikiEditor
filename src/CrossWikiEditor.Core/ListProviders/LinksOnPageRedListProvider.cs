using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class LinksOnPageRedListProvider : LinksOnPageListProvider
{
    public LinksOnPageRedListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService) : base(dialogService, pageService, settingsService)
    {
    }

    public override string Title => "Links on page (only redlinks)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        Result<List<WikiPageModel>> result = await base.MakeList(limit);
        if (result is not {IsSuccessful: true, Value: not null})
        {
            return result;
        }

        bool[] existsTable = await Task.WhenAll(result.Value.Select(p => p.Exists()));
        return result.Value.Where((_, index) => !existsTable[index]).ToList();
    }
}
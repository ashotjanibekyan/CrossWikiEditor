using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class WikiSearchInTextListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Wiki search (text)";
    public override string ParamTitle => "Wiki search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.WikiSearch(userPreferencesService.CurrentApiUrl, Param, new[] {0}, limit);
}
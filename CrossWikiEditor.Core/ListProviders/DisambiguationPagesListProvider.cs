using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class DisambiguationPagesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    private int[]? _namespaces;

    public override string Title => "Disambiguation pages";
    public override string ParamTitle => "";
    public override bool CanMake => _namespaces is {Length: > 0};
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetPagesWithProp(userPreferencesService.GetCurrentPref().UrlApi(), "disambiguation", limit);
    }
}
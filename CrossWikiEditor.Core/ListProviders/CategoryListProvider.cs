using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    public override string Title => "Category";
    public override string ParamTitle => "Category";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetPagesOfCategory(userPreferencesService.CurrentApiUrl, Param, limit);
}
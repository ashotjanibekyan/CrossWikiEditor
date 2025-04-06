using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoriesOnPageOnlyHiddenCategoriesListProvider : CategoriesOnPageListProvider
{
    public CategoriesOnPageOnlyHiddenCategoriesListProvider(ICategoryService categoryService,
        IDialogService dialogService,
        ISettingsService settingsService) : base(categoryService, dialogService, settingsService)
    {
    }

    public override string Title => "Categories on page (only hidden categories)";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await CategoryService.GetCategoriesOf(SettingsService.CurrentApiUrl, Param, limit, onlyHidden: true);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryRecursive1LevelListProvider : LimitedListProviderBase
{
    private readonly ICategoryService _categoryService;
    private readonly ISettingsService _settingsService;

    public CategoryRecursive1LevelListProvider(ICategoryService categoryService,
        IDialogService dialogService,
        ISettingsService settingsService) : base(dialogService)
    {
        _categoryService = categoryService;
        _settingsService = settingsService;
    }

    public override string Title => "Category (recursive 1 level)";
    public override string ParamTitle => "Category";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _categoryService.GetPagesOfCategory(_settingsService.CurrentApiUrl, Param, limit, 1);
    }
}
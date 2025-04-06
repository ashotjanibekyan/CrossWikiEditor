using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public class CategoriesOnPageListProvider : LimitedListProviderBase
{
    private readonly ICategoryService _categoryService;
    private readonly ISettingsService _settingsService;

    public CategoriesOnPageListProvider(ICategoryService categoryService,
        IDialogService dialogService,
        ISettingsService settingsService) : base(dialogService)
    {
        _categoryService = categoryService;
        _settingsService = settingsService;
    }

    protected ISettingsService SettingsService => _settingsService;
    protected ICategoryService CategoryService => _categoryService;
    public override string Title => "Categories on page";
    public override string ParamTitle => "Page";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await _categoryService.GetCategoriesOf(_settingsService.CurrentApiUrl, Param, limit);
    }
}
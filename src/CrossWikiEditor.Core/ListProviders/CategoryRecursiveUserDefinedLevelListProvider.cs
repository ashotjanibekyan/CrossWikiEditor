using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.ViewModels;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class CategoryRecursiveUserDefinedLevelListProvider : LimitedListProviderBase, INeedAdditionalParamsListProvider
{
    private int? _recursionLevel;
    private readonly ICategoryService _categoryService;
    private readonly ISettingsService _settingsService;

    public CategoryRecursiveUserDefinedLevelListProvider(ICategoryService categoryService,
        IDialogService dialogService,
        ISettingsService settingsService) : base(dialogService)
    {
        _categoryService = categoryService;
        _settingsService = settingsService;
    }

    public override string Title => "Category (recursive user defined level)";
    public override string ParamTitle => "Category";
    public override bool CanMake => _recursionLevel is not null && !string.IsNullOrWhiteSpace(Param);

    public async Task GetAdditionalParams()
    {
        int? result = await DialogService.ShowDialog<int?>(new PromptViewModel("Number", "Recursion depth: ")
        {
            IsNumeric = true,
            Value = _recursionLevel ?? 1
        });
        if (result is not null)
        {
            _recursionLevel = result;
        }
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        if (_recursionLevel is null)
        {
            return new Exception("Please select recursive level.");
        }

        UserSettings userSettings = _settingsService.GetCurrentSettings();
        Result<List<WikiPageModel>> result = await _categoryService.GetPagesOfCategory(userSettings.GetApiUrl(), Param, limit, (int) _recursionLevel);
        _recursionLevel = null;
        return result;
    }
}
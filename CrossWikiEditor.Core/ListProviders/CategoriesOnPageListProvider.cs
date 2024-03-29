﻿namespace CrossWikiEditor.Core.ListProviders;

public class CategoriesOnPageListProvider(ICategoryService categoryService,
    IDialogService dialogService, IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService)
{
    protected IUserPreferencesService UserPreferencesService => userPreferencesService;
    protected ICategoryService CategoryService => categoryService;
    public override string Title => "Categories on page";
    public override string ParamTitle => "Page";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await categoryService.GetCategoriesOf(userPreferencesService.CurrentApiUrl, Param, limit);
}
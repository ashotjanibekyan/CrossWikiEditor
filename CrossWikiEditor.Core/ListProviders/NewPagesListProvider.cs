﻿namespace CrossWikiEditor.Core.ListProviders;

public sealed class NewPagesListProvider(
    IUserPreferencesService userPreferencesService,
    IPageService pageService,
    IViewModelFactory viewModelFactory,
    IDialogService dialogService) : LimitedListProviderBase(dialogService), INeedNamespacesListProvider
{
    private int[]? _namespaces;
    public override string Title => "New pages";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _namespaces is {Length: > 0};
    
    public async Task GetAdditionalParams() => _namespaces = await this.GetNamespaces(true, dialogService, viewModelFactory);

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await pageService.GetNewPages(userPreferencesService.CurrentApiUrl, _namespaces!, limit);
}
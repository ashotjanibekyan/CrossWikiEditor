using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Core.ListProviders;

public class PagesWithoutLanguageLinksListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IViewModelFactory viewModelFactory,
    IUserPreferencesService userPreferencesService) : AllPagesListProviderBase(dialogService, pageService, viewModelFactory, userPreferencesService)
{
    public override string Title => "Pages without language links";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await MakeListBase(limit, PropertyFilterOption.Disable, PropertyFilterOption.WithoutProperty);
}
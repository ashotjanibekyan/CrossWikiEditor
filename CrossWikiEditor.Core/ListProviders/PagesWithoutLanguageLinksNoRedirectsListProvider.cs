using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Core.ListProviders;

public class PagesWithoutLanguageLinksNoRedirectsListProvider(
    IDialogService dialogService, 
    IViewModelFactory viewModelFactory, 
    IUserPreferencesService userPreferencesService, 
    IPageService pageService) : AllPagesListProviderBase(dialogService, pageService, viewModelFactory, userPreferencesService)
{
    public override string Title => "Pages without language links (no redirects)";
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit) =>
        await MakeListBase(limit, PropertyFilterOption.WithoutProperty, PropertyFilterOption.WithoutProperty);
}
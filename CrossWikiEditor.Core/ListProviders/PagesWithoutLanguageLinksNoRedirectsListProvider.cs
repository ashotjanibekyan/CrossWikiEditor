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
    IPageService pageService) : PagesWithoutLanguageLinksListProvider(dialogService, viewModelFactory, userPreferencesService, pageService)
{
    public override string Title => "Pages without language links (no redirects)";
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        string apiRoot = userPreferencesService.GetCurrentPref().UrlApi();
        return await pageService.GetAllPages(apiRoot, Param, _namespaces!.First(), PropertyFilterOption.WithoutProperty, PropertyFilterOption.WithoutProperty, limit);
    }
}
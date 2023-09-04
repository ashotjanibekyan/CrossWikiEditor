using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Core.Services.WikiServices;

public interface IPageService
{
    Task<Result<List<WikiPageModel>>> GetCategoriesOf(string apiRoot, string pageName, int limit, bool includeHidden = true, bool onlyHidden = false);
    Task<Result<List<WikiPageModel>>> GetPagesOfCategory(string apiRoot, string categoryName, int limit, int recursive = 0);
    Task<Result<List<WikiPageModel>>> FilesOnPage(string apiRoot, string pageName, int limit);

    /// <summary>
    /// Gets random pages in a specific namespace
    /// </summary>
    /// <param name="apiRoot">The api root of the wiki, e.g. https://hy.wikipedia.org/w/api.php?</param>
    /// <param name="namespaces">Only list pages in these namespaces. Should be null if all the namespaces are selected.</param>
    /// <param name="limit">Number of pages to generate.</param>
    /// <returns></returns>
    Task<Result<List<WikiPageModel>>> GetRandomPages(string apiRoot, int[]? namespaces, int limit);

    Task<Result<List<WikiPageModel>>> GetPagesByFileUsage(string apiRoot, string fileName, int limit);
    Task<Result<List<WikiPageModel>>> LinksOnPage(string apiRoot, string pageName, int limit);
    Task<Result<List<WikiPageModel>>> GetNewPages(string apiRoot, int limit);
    Task<Result<List<WikiPageModel>>> GetTransclusionsOn(string apiRoot, string pageName, int limit);
    Task<Result<List<WikiPageModel>>> GetTransclusionsOf(string apiRoot, string pageName, int[]? namespaces, int limit);

    Task<Result<List<WikiPageModel>>> GetPagesLinkedTo(string apiRoot, string title, int[]? namespaces, bool allowRedirectLinks,
        bool? filterRedirects, int limit);

    Task<Result<List<WikiPageModel>>> GetPagesWithProp(string apiRoot, string param, int limit);
    Task<Result<List<WikiPageModel>>> GetAllCategories(string apiRoot, string startTitle, int limit);
    Task<Result<List<WikiPageModel>>> GetAllFiles(string apiRoot, string startTitle, int limit);
    Task<Result<List<WikiPageModel>>> GetAllPages(string apiRoot, string startTitle, int namespaceId, PropertyFilterOption redirectsFilter, int limit);
    Task<Result<List<WikiPageModel>>> GetAllPagesWithPrefix(string apiRoot, string prefix, int namespaceId, int limit);
    Task<Result<List<WikiPageModel>>> WikiSearch(string apiRoot, string keyword, int[]? namespaces, int limit);

    Result<List<WikiPageModel>> ConvertToTalk(List<WikiPageModel> pages);
    Result<List<WikiPageModel>> ConvertToSubject(List<WikiPageModel> pages);
}
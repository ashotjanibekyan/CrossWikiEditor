namespace CrossWikiEditor.Core.Services.WikiServices;

public interface IPageService
{
    Task<Result<List<WikiPageModel>>> FilesOnPage(string apiRoot, string pageName, int limit);
    Task<Result<List<WikiPageModel>>> GetRandomPages(string apiRoot, int[]? namespaces, bool? filterRedirects, int limit);
    Task<Result<List<WikiPageModel>>> GetPagesByFileUsage(string apiRoot, string fileName, int limit);
    Task<Result<List<WikiPageModel>>> LinksOnPage(string apiRoot, string pageName, int limit);
    Task<Result<List<WikiPageModel>>> GetNewPages(string apiRoot, int[] namespaces, int limit);
    Task<Result<List<WikiPageModel>>> GetTransclusionsOn(string apiRoot, string pageName, int limit);
    Task<Result<List<WikiPageModel>>> GetTransclusionsOf(string apiRoot, string pageName, int[]? namespaces, int limit);
    Task<Result<List<WikiPageModel>>> GetPagesLinkedTo(string apiRoot, string title, int[]? namespaces, bool allowRedirectLinks, bool? filterRedirects, int limit);
    Task<Result<List<WikiPageModel>>> GetPagesWithProp(string apiRoot, string param, int limit);
    Task<Result<List<WikiPageModel>>> GetAllFiles(string apiRoot, string startTitle, int limit);
    Task<Result<List<WikiPageModel>>> GetAllPages(string apiRoot, string startTitle, int namespaceId, PropertyFilterOption redirectsFilter, PropertyFilterOption langLinksFilter, int limit);
    Task<Result<List<WikiPageModel>>> GetAllPagesWithPrefix(string apiRoot, string prefix, int namespaceId, int limit);
    Task<Result<List<WikiPageModel>>> GetProtectedPages(string apiRoot, string protectType, string protectLevel, int limit);
    Task<Result<List<WikiPageModel>>> WikiSearch(string apiRoot, string keyword, int[] namespaces, int limit);
    Task<Result<List<WikiPageModel>>> GetRecentlyChangedPages(string apiRoot, int[]? namespaces, int limit);
    Task<Result<List<WikiPageModel>>> LinkSearch(string apiRoot, string url, int limit);

    Result<List<WikiPageModel>> ConvertToTalk(List<WikiPageModel> pages);
    Result<List<WikiPageModel>> ConvertToSubject(List<WikiPageModel> pages);
}
namespace CrossWikiEditor.Core.Services.WikiServices;

public interface ICategoryService
{
    Task<Result<List<WikiPageModel>>> GetCategoriesOf(string apiRoot, string pageName, int limit, bool includeHidden = true, bool onlyHidden = false);
    Task<Result<List<WikiPageModel>>> GetPagesOfCategory(string apiRoot, string categoryName, int limit, int recursive = 0);
    Task<Result<List<WikiPageModel>>> GetAllCategories(string apiRoot, string startTitle, int limit);
}
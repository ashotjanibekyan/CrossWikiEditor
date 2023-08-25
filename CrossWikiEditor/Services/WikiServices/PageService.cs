using System.Collections.Generic;
using System.Threading.Tasks;
using WikiClient;

namespace CrossWikiEditor.Services.WikiServices;

public interface IPageService
{
    Task<Result<List<string>>> GetCategoriesOf(Site site, string page, bool includeHidden=true, bool onlyHidden=false);
    Task<Result<List<string>>> GetPagesOfCategory(Site site, string categoryName, int recursive = 0);
}

public sealed class PageService : IPageService
{
    public async Task<Result<List<string>>> GetCategoriesOf(Site site, string page, bool includeHidden=true, bool onlyHidden=false)
    {
        await Task.Delay(100);
        return Result<List<string>>.Success(new List<string>
        {
            "Page1",
            "Page2"
        });
    }
    
    public async Task<Result<List<string>>> GetPagesOfCategory(Site site, string categoryName, int recursive = 0)
    {
        await Task.Delay(100);
        return Result<List<string>>.Success(new List<string>
        {
            "Page1",
            "Page2"
        });
    }
}

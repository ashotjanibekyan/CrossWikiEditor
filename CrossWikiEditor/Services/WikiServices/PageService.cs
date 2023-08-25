using System.Collections.Generic;
using System.Threading.Tasks;
using WikiClient;

namespace CrossWikiEditor.Services.WikiServices;

public interface IPageService
{
    Task<Result<List<string>>> GetCategoriesOf(Site site, string page);
}

public sealed class PageService : IPageService
{
    public async Task<Result<List<string>>> GetCategoriesOf(Site site, string page)
    {
        await Task.Delay(100);
        return Result<List<string>>.Success(new List<string>
        {
            "Page1",
            "Page2"
        });
    }
}

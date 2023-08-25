using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikiClientLibrary;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public interface IPageService
{
    Task<Result<List<string>>> GetCategoriesOf(string apiRoot, string page, bool includeHidden = true, bool onlyHidden = false);
    Task<Result<List<string>>> GetPagesOfCategory(string apiRoot, string categoryName, int recursive = 0);
}

public sealed class PageService : IPageService
{
    public async Task<Result<List<string>>> GetCategoriesOf(string apiRoot, string page, bool includeHidden = true, bool onlyHidden = false)
    {
        try
        {
            if (!includeHidden && onlyHidden)
            {
                return Result<List<string>>.Success(new List<string>());
            }
            WikiSite site = await WikiClientCache.GetWikiSite(apiRoot);
            var catGen = new CategoriesGenerator(site, page)
            {
                HiddenCategoryFilter = PropertyFilterOption.Disable
            };
            if (!includeHidden)
            {
                catGen.HiddenCategoryFilter = PropertyFilterOption.WithoutProperty;
            }
            if (onlyHidden)
            {
                catGen.HiddenCategoryFilter = PropertyFilterOption.WithProperty;
            }

            List<WikiPage> result = await catGen.EnumPagesAsync().ToListAsync();
            return Result<List<string>>.Success(result.Select(x => x.Title).ToList());
        }
        catch (Exception e)
        {
            return Result<List<string>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<string>>> GetPagesOfCategory(string apiRoot, string categoryName, int recursive = 0)
    {
        try
        {
            List<List<WikiPage>> resultByDepth = new();
            WikiSite site = await WikiClientCache.GetWikiSite(apiRoot);
            var catGen = new CategoryMembersGenerator(new WikiPage(site, categoryName));
            resultByDepth.Add(await catGen.EnumPagesAsync().ToListAsync());

            for (int i = 0; i < recursive; i++)
            {
                var subCats = resultByDepth[i].Where(x => x.NamespaceId == BuiltInNamespaces.Category).ToList();
                var temp = new List<WikiPage>();
                foreach (WikiPage subCat in subCats)
                {
                    catGen = new CategoryMembersGenerator(new WikiPage(site, subCat.Title));
                    temp.AddRange(await catGen.EnumPagesAsync().ToListAsync());
                }
                resultByDepth.Add(temp);
            }
            
            return Result<List<string>>.Success(resultByDepth.SelectMany(list => list).Select(x => x.Title).ToList());
        }
        catch (Exception e)
        {
            return Result<List<string>>.Failure(e.Message);
        }
    }
}
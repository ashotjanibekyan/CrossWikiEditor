namespace CrossWikiEditor.Core.Services.WikiServices;

public sealed class CategoryService(IWikiClientCache wikiClientCache, ILogger logger) : ICategoryService
{
    public async Task<Result<List<WikiPageModel>>> GetCategoriesOf(string apiRoot, string pageName, int limit, bool includeHidden = true,
        bool onlyHidden = false)
    {
        try
        {
            if (!includeHidden && onlyHidden)
            {
                return new List<WikiPageModel>();
            }

            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var catGen = new CategoriesGenerator(site, pageName)
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

            List<WikiPage> result = await catGen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site: {Site}, page: {Page}, includeHidden: {IncludeHidden}, onlyHidden: {OnlyHidden}, limit: {Limit}",
                apiRoot, pageName, includeHidden, onlyHidden, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetPagesOfCategory(string apiRoot, string categoryName, int limit, int recursive = 0)
    {
        if (!categoryName.Contains(':'))
        {
            categoryName = $"Category:{categoryName}";
        }

        try
        {
            List<List<WikiPage>> resultByDepth = [];
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var catGen = new CategoryMembersGenerator(new WikiPage(site, categoryName));
            resultByDepth.Add(await catGen.EnumPagesAsync().Take(limit).ToListAsync());

            for (int i = 0; i < recursive; i++)
            {
                var subCats = resultByDepth[i].Where(x => x.NamespaceId == BuiltInNamespaces.Category).ToList();
                var temp = new List<WikiPage>();
                foreach (WikiPage subCat in subCats)
                {
                    if (subCat.Title is null)
                    {
                        continue;
                    }
                    catGen = new CategoryMembersGenerator(new WikiPage(site, subCat.Title));
                    temp.AddRange(await catGen.EnumPagesAsync().Take(limit).Take(limit).ToListAsync());
                }

                resultByDepth.Add(temp);
            }

            return resultByDepth.SelectMany(list => list).Select(x => new WikiPageModel(x)).ToList();
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Category: {CategoryName}, recursive: {Recursive}, limit: {Limit}", categoryName, recursive, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetAllCategories(string apiRoot, string startTitle, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new AllCategoriesGenerator(site)
            {
                StartTitle = startTitle
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, start title: {StartTitle}, limit: {Limit}", apiRoot, startTitle, limit);
            return e;
        }
    }
}
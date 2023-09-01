using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Utils;
using CrossWikiEditor.WikiClientLibraryUtils.Generators;
using Serilog;
using WikiClientLibrary;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public interface IPageService
{
    Task<Result<List<WikiPageModel>>> GetCategoriesOf(string apiRoot, string pageName, bool includeHidden = true, bool onlyHidden = false);
    Task<Result<List<WikiPageModel>>> GetPagesOfCategory(string apiRoot, string categoryName, int recursive = 0);
    Task<Result<List<WikiPageModel>>> FilesOnPage(string apiRoot, string pageName);

    /// <summary>
    /// Gets random pages in a specific namespace
    /// </summary>
    /// <param name="apiRoot">The api root of the wiki, e.g. https://hy.wikipedia.org/w/api.php?</param>
    /// <param name="numberOfPages">Number of pages to generate.</param>
    /// <param name="namespaces">Only list pages in these namespaces. Should be null if all the namespaces are selected.</param>
    /// <returns></returns>
    Task<Result<List<WikiPageModel>>> GetRandomPages(string apiRoot, int numberOfPages, int[]? namespaces);

    Task<Result<List<WikiPageModel>>> GetPagesByFileUsage(string apiRoot, string fileName);
    Task<Result<List<WikiPageModel>>> LinksOnPage(string apiRoot, string pageName);
    Task<Result<List<WikiPageModel>>> GetNewPages(string apiRoot);
    Task<Result<List<WikiPageModel>>> GetTransclusionsOn(string apiRoot, string pageName);
    Task<Result<List<WikiPageModel>>> GetTransclusionsOf(string apiRoot, string pageName, int[]? namespaces);

    Task<Result<List<WikiPageModel>>> GetPagesLinkedTo(string apiRoot, string title, int[]? namespaces, bool allowRedirectLinks,
        bool? filterRedirects);

    Task<Result<List<WikiPageModel>>> WikiSearch(string apiRoot, string keyword, int[]? namespaces);

    Result<List<WikiPageModel>> ConvertToTalk(List<WikiPageModel> pages);
    Result<List<WikiPageModel>> ConvertToSubject(List<WikiPageModel> pages);
}

public sealed class PageService(IWikiClientCache wikiClientCache, IUserPreferencesService userPreferencesService, ILogger logger)
    : IPageService
{
    public async Task<Result<List<WikiPageModel>>> GetCategoriesOf(string apiRoot, string pageName, bool includeHidden = true,
        bool onlyHidden = false)
    {
        try
        {
            if (!includeHidden && onlyHidden)
            {
                return Result<List<WikiPageModel>>.Success(new List<WikiPageModel>());
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

            List<WikiPage> result = await catGen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site: {Site}, page: {Page}, includeHidden: {IncludeHidden}, onlyHidden: {OnlyHidden}", 
                apiRoot, pageName, includeHidden, onlyHidden);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetPagesOfCategory(string apiRoot, string categoryName, int recursive = 0)
    {
        if (!categoryName.Contains(':'))
        {
            categoryName = $"Category:{categoryName}";
        }

        try
        {
            List<List<WikiPage>> resultByDepth = new();
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
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

            return Result<List<WikiPageModel>>.Success(resultByDepth.SelectMany(list => list).Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Category: {CategoryName}, recursive: {Recursive}", categoryName, recursive);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> FilesOnPage(string apiRoot, string pageName)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new FilesGenerator(site, pageName);
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, page: {Page}", apiRoot, pageName);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetRandomPages(string apiRoot, int numberOfPages, int[]? namespaces)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new RandomPageGenerator(site)
            {
                NamespaceIds = namespaces,
                PaginationSize = numberOfPages
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(numberOfPages).ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Root}, numberOfPages: {NumberOfPages}, namespaces: {Namespaces}", apiRoot, numberOfPages,
                namespaces);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetPagesByFileUsage(string apiRoot, string fileName)
    {
        try
        {
            if (!fileName.Contains(':'))
            {
                fileName = $"File:{fileName}";
            }

            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new FileUsageGenerator(site, fileName);
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, file: {File}", apiRoot, fileName);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> LinksOnPage(string apiRoot, string pageName)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new LinksGenerator(site, pageName);
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, page: {Page}", apiRoot, pageName);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetNewPages(string apiRoot)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new RecentChangesGenerator(site)
            {
                TypeFilters = RecentChangesFilterTypes.Create,
                RedirectsFilter = PropertyFilterOption.WithoutProperty,
                NamespaceIds = new[] { 0 }
            };
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}", apiRoot);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetTransclusionsOn(string apiRoot, string pageName)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new TransclusionsGenerator(site, pageName)
            {
                PaginationSize = 500
            };
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, page: {Page}", apiRoot, pageName);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetTransclusionsOf(string apiRoot, string pageName, int[]? namespaces)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new TranscludedInGenerator(site, pageName)
            {
                PaginationSize = 500,
                NamespaceIds = namespaces
            };
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, page: {Page}, namespaces: {Namespaces}", apiRoot, pageName, namespaces);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetPagesLinkedTo(string apiRoot,
        string title,
        int[]? namespaces,
        bool allowRedirectLinks,
        bool? filterRedirects)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new BacklinksGenerator(site, title)
            {
                PaginationSize = 500,
                NamespaceIds = namespaces,
                RedirectsFilter = filterRedirects switch
                {
                    true => PropertyFilterOption.WithProperty,
                    false => PropertyFilterOption.WithoutProperty,
                    null => PropertyFilterOption.Disable
                },
                AllowRedirectedLinks = allowRedirectLinks
            };
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, title: {Title}, namespaces: {Namespaces}, allowRedirectLinks: {AllowRedirectLinks}, filterRedirects: {FilterRedirects}", 
                apiRoot, title, namespaces, allowRedirectLinks, filterRedirects);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> WikiSearch(string apiRoot, string keyword, int[]? namespaces)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new SearchGenerator(site, keyword)
            {
                NamespaceIds = namespaces
            };
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, keyword: {Keyword}, namespaces: {Namespaces}", apiRoot, keyword, namespaces);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    private Result<WikiPageModel> ConvertToTalk(WikiPageModel page)
    {
        try
        { 
            return Result<WikiPageModel>.Success(page.ToTalkPage());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. {WikiPageModel}", page);
            return Result<WikiPageModel>.Failure(e.Message);
        }
    }

    public Result<List<WikiPageModel>> ConvertToTalk(List<WikiPageModel> pages)
    {
        List<WikiPageModel> result = new();
        foreach (WikiPageModel wikiPageModel in pages)
        {
            Result<WikiPageModel> talkPageResult = ConvertToTalk(wikiPageModel);
            if (talkPageResult is { IsSuccessful: true, Value: not null })
            {
                result.Add(talkPageResult.Value);
            }
        }

        return Result<List<WikiPageModel>>.Success(result);
    }

    private Result<WikiPageModel> ConvertToSubject(WikiPageModel page)
    {
        try
        {
            return Result<WikiPageModel>.Success(page.ToSubjectPage());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. {WikiPageModel}", page);
            return Result<WikiPageModel>.Failure(e.Message);
        }
    }

    public Result<List<WikiPageModel>> ConvertToSubject(List<WikiPageModel> pages)
    {
        List<WikiPageModel> result = (from wikiPageModel in pages
            select ConvertToSubject(wikiPageModel)
            into subjectPageResult
            where subjectPageResult is {IsSuccessful: true, Value: not null}
            select subjectPageResult.Value).ToList();

        return Result<List<WikiPageModel>>.Success(result);
    }
}
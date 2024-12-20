﻿namespace CrossWikiEditor.Core.Services.WikiServices;

public sealed class PageService(IWikiClientCache wikiClientCache, ILogger logger)
    : IPageService
{
    public async Task<Result<List<WikiPageModel>>> FilesOnPage(string apiRoot, string pageName, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new FilesGenerator(site, pageName);
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, page: {Page}, limit: {Limit}", apiRoot, pageName, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetRandomPages(string apiRoot, int[]? namespaces, bool? filterRedirects, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new RandomPageGenerator(site)
            {
                NamespaceIds = namespaces,
                PaginationSize = limit,
                RedirectsFilter = filterRedirects switch
                {
                    true => PropertyFilterOption.WithProperty,
                    false => PropertyFilterOption.WithoutProperty,
                    null => PropertyFilterOption.Disable
                }
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Root}, namespaces: {Namespaces}, limit: {Limit}", apiRoot, namespaces, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetPagesByFileUsage(string apiRoot, string fileName, int limit)
    {
        try
        {
            if (!fileName.Contains(':'))
            {
                fileName = $"File:{fileName}";
            }

            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new FileUsageGenerator(site, fileName);
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, file: {File}, limit: {Limit}", apiRoot, fileName, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> LinksOnPage(string apiRoot, string pageName, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new LinksGenerator(site, pageName);
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, page: {Page}, limit: {Limit}", apiRoot, pageName, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetNewPages(string apiRoot, int[] namespaces, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new RecentChangesGenerator(site)
            {
                TypeFilters = RecentChangesFilterTypes.Create,
                RedirectsFilter = PropertyFilterOption.WithoutProperty,
                NamespaceIds = namespaces
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, limit: {Limit}", apiRoot, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetTransclusionsOn(string apiRoot, string pageName, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new TransclusionsGenerator(site, pageName)
            {
                PaginationSize = 500
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, page: {Page}, limit: {Limit}", apiRoot, pageName, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetTransclusionsOf(string apiRoot, string pageName, int[]? namespaces, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new TranscludedInGenerator(site, pageName)
            {
                PaginationSize = 500,
                NamespaceIds = namespaces
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, page: {Page}, namespaces: {Namespaces}, limit: {Limit}", apiRoot, pageName, namespaces,
                limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetPagesLinkedTo(string apiRoot,
        string title,
        int[]? namespaces,
        bool allowRedirectLinks,
        bool? filterRedirects,
        int limit)
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
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e,
                "Failed to get pages. Site {Site}, title: {Title}, namespaces: {Namespaces}, allowRedirectLinks: {AllowRedirectLinks}, filterRedirects: {FilterRedirects}, limit: {Limit}",
                apiRoot, title, namespaces, allowRedirectLinks, filterRedirects, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetPagesWithProp(string apiRoot, string param, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new PagesWithPropGenerator(site, param);
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, property name: {Keyword}, limit: {Limit}", apiRoot, param, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetAllFiles(string apiRoot, string startTitle, int limit)
    {
        return await GetAllPages(
            apiRoot,
            startTitle,
            6,
            PropertyFilterOption.Disable,
            PropertyFilterOption.Disable,
            limit);
    }

    public async Task<Result<List<WikiPageModel>>> GetAllPages(string apiRoot, string startTitle, int namespaceId,
        PropertyFilterOption redirectsFilter, PropertyFilterOption langLinksFilter, int limit)
    {
        return await GetAllPages(apiRoot, namespaceId, redirectsFilter, langLinksFilter, limit, startTitle);
    }

    public async Task<Result<List<WikiPageModel>>> GetAllPagesWithPrefix(string apiRoot, string prefix, int namespaceId, int limit)
    {
        return await GetAllPages(apiRoot, namespaceId, PropertyFilterOption.Disable, PropertyFilterOption.Disable, limit, prefix: prefix);
    }

    public async Task<Result<List<WikiPageModel>>> GetProtectedPages(string apiRoot, string protectType, string protectLevel, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new AllPagesGeneratorEx(site)
            {
                PaginationSize = 500,
                ProtectionType = protectType,
                ProtectionLevel = protectLevel
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, protectType: {ProtectType}, protectLevel: {ProtectLevel}, limit: {Limit}", apiRoot,
                protectType, protectLevel, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> WikiSearch(string apiRoot, string keyword, int[] namespaces, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new SearchGenerator(site, keyword)
            {
                NamespaceIds = namespaces,
                PaginationSize = Math.Min(limit, 500)
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site {Site}, keyword: {Keyword}, namespaces: {Namespaces}, limit: {Limit}", apiRoot, keyword,
                namespaces, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetRecentlyChangedPages(string apiRoot, int[]? namespaces, int limit)
    {
        try
        {
            WikiSite wikiSite = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new RecentChangesGenerator(wikiSite)
            {
                NamespaceIds = namespaces
            };
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site: {ApiRoot}, namespaces: {Namespaces}, limit: {Limit}", apiRoot, namespaces, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> LinkSearch(string apiRoot, string url, int limit)
    {
        try
        {
            WikiSite wikiSite = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new ExternalUrlUsageGenerator(wikiSite)
            {
                Url = url
            };

            List<ExternalUrlUsageItem> result = await gen.EnumItemsAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(new WikiPage(wikiSite, x.Title, x.NamespaceId)));
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. Site: {ApiRoot}, Url: {Url} limit: {Limit}", apiRoot, url, limit);
            return e;
        }
    }

    public Result<List<WikiPageModel>> ConvertToSubject(List<WikiPageModel> pages)
    {
        List<WikiPageModel> result = (from wikiPageModel in pages
            select ConvertToSubject(wikiPageModel)
            into subjectPageResult
            where subjectPageResult is {IsSuccessful: true, Value: not null}
            select subjectPageResult.Value).ToList();

        return result;
    }

    public Result<List<WikiPageModel>> ConvertToTalk(List<WikiPageModel> pages)
    {
        var result = (from wikiPageModel in pages
            select ConvertToTalk(wikiPageModel)
            into talkPageResult
            where talkPageResult is {IsSuccessful: true, Value: not null}
            select talkPageResult.Value).ToList();

        return result;
    }

    private async Task<Result<List<WikiPageModel>>> GetAllPages(
        string apiRoot,
        int namespaceId,
        PropertyFilterOption redirectsFilter,
        PropertyFilterOption langLinksFilter,
        int limit,
        string? startTitle = null,
        string? prefix = null)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new AllPagesGenerator(site)
            {
                NamespaceId = namespaceId,
                RedirectsFilter = redirectsFilter,
                LanguageLinkFilter = langLinksFilter,
                PaginationSize = 500
            };
            if (startTitle is not null)
            {
                gen.StartTitle = startTitle;
            }

            if (prefix is not null)
            {
                gen.Prefix = prefix;
            }

            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            logger.Fatal(e,
                "Failed to get pages. Site {Site}, start title: {StartTitle}, prefix: {Prefix}, namespace: {NamespaceId}, redirectsFilter {RedirectsFilter}, limit: {Limit}",
                apiRoot,
                startTitle, prefix, namespaceId, redirectsFilter, limit);
            return e;
        }
    }

    private Result<WikiPageModel> ConvertToSubject(WikiPageModel page)
    {
        try
        {
            return page.ToSubjectPage();
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. {WikiPageModel}", page);
            return e;
        }
    }

    private Result<WikiPageModel> ConvertToTalk(WikiPageModel page)
    {
        try
        {
            return page.ToTalkPage();
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get pages. {WikiPageModel}", page);
            return e;
        }
    }
}
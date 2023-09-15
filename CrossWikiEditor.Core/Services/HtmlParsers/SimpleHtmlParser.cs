namespace CrossWikiEditor.Core.Services.HtmlParsers;

public sealed class SimpleHtmlParser(ILogger logger, IUserPreferencesService userPreferencesService,
    IWikiClientCache wikiClientCache)
{
    readonly char[] _terminationChars = {' ', '\t', '\n', '"', '<', '>', '{', '}', '&'};
    
    public async Task<List<WikiPageModel>> GetPages(string html)
    {
        string[] results = html.Split(userPreferencesService.GetCurrentPref().UrlBase());
        return await GetWikiPageModels(results);
    }
    
    private async Task<List<WikiPageModel>> GetWikiPageModels(string[] results)
    {
        var wikiPageModels = new List<WikiPageModel>();
        foreach (string urlStart in results.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            try
            {
                wikiPageModels.Add(await TryGetWikiPageModel(urlStart));
            }
            catch (Exception ex)
            {
                logger.Debug(ex, $"{nameof(SimpleHtmlParser)} failed to parse a URL");
            }
        }

        return wikiPageModels;
    }

    private async Task<WikiPageModel> TryGetWikiPageModel(string urlStart)
    {
        string baseUrl = userPreferencesService.GetCurrentPref().UrlBase();
        WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.CurrentApiUrl);
        string url = urlStart;
        int i = urlStart.IndexOfAny(_terminationChars);
        if (i != -1)
        {
            url = urlStart[..i];
        }
        
        if (url.Last() == '\'' || url.Last() == '\"')
        {
            var page = new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url[..^1])));
            if (await page.Exists())
            {
                return page;
            }
            return new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url)));
        }
        return new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url)));
    }
}
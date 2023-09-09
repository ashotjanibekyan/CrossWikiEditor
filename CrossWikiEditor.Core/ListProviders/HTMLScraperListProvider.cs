namespace CrossWikiEditor.Core.ListProviders;

public sealed class HtmlScraperListProvider(ILogger logger, IUserPreferencesService userPreferencesService,
    IWikiClientCache wikiClientCache) : UnlimitedListProviderBase
{
    public override string Title => "HTML Scraper";
    public override string ParamTitle => "URL";

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            string baseUrl = userPreferencesService.GetCurrentPref().UrlBase();
            WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.CurrentApiUrl);
            var httpClient = new HttpClient();
            string html = await httpClient.GetStringAsync(Param);

            var urls = new List<WikiPageModel>();
            urls.AddRange(await TerminationCharsBasedParser(html, baseUrl, site));
            urls.AddRange(HtmlAgilityPackBasedParser(html, baseUrl, site));
            return Result<List<WikiPageModel>>.Success(urls.Distinct().ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, $"{nameof(HtmlScraperListProvider)} failed to make a list");
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    private async Task<List<WikiPageModel>> TerminationCharsBasedParser(string html, string baseUrl, WikiSite site)
    {
        char[] terminationChars = new[] {' ', '\t', '\n', '"', '<', '>', '{', '}', '&'};

        string[] results = html.Split(baseUrl);
        var wikiPageModels = new List<WikiPageModel>();

        foreach (string urlStart in results)
        {
            if (urlStart == string.Empty)
            {
                continue;
            }

            string url = urlStart;
            int i = urlStart.IndexOfAny(terminationChars);
            if (i != -1)
            {
                url = urlStart[..i];
            }

            try
            {
                if (url.Last() == '\'' || url.Last() == '\"')
                {
                    var page = new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url[..^1])));
                    if (await page.Exists())
                    {
                        wikiPageModels.Add(page);
                    }
                    else
                    {
                        wikiPageModels.Add(new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url))));
                    }
                }
                else
                {
                    wikiPageModels.Add(new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url))));
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex, $"{nameof(TerminationCharsBasedParser)} failed to parse a URL");
            }
        }

        return wikiPageModels;
    }

    private List<WikiPageModel> HtmlAgilityPackBasedParser(string html, string baseUrl, WikiSite site)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var urls = new List<WikiPageModel>();

        foreach (HtmlNode? link in doc.DocumentNode.SelectNodes("//a[@href]"))
        {
            try
            {
                string? hrefValue = link.GetAttributeValue("href", string.Empty);

                if (hrefValue.Contains(baseUrl))
                {
                    urls.Add(new WikiPageModel(new WikiPage(site,
                        Tools.GetPageTitleFromUrl(hrefValue[hrefValue.IndexOf(baseUrl, StringComparison.Ordinal)..]))));
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex, $"{nameof(HtmlAgilityPackBasedParser)} failed to parse a URL");
            }
        }

        return urls;
    }
}
namespace CrossWikiEditor.Core.Services.HtmlParsers;

public sealed class HtmlAgilityPackParser(ILogger logger, IUserPreferencesService userPreferencesService, IWikiClientCache wikiClientCache)
{
    public async Task<List<WikiPageModel>> GetPages(string html)
    {
        string baseUrl = userPreferencesService.GetCurrentPref().UrlBase();
        WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.CurrentApiUrl);
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
                logger.Debug(ex, $"{nameof(HtmlAgilityPackParser)} failed to parse a URL");
            }
        }

        return urls;
    }
}
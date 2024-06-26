namespace CrossWikiEditor.Core.Services.HtmlParsers;

public sealed class HtmlAgilityPackParser(ILogger logger, ISettingsService settingsService, IWikiClientCache wikiClientCache)
{
    public List<WikiPageModel> GetPages(string html)
    {
        string baseUrl = settingsService.GetCurrentSettings().GetBaseUrl();
        string apiUrl = settingsService.GetCurrentSettings().GetApiUrl();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var urls = new List<WikiPageModel>();
        HtmlNodeCollection? nodes = doc.DocumentNode.SelectNodes("//a[@href]");
        if (nodes is null)
        {
            return urls;
        }

        foreach (HtmlNode? link in nodes)
        {
            try
            {
                string? hrefValue = link.GetAttributeValue("href", string.Empty);

                if (hrefValue.Contains(baseUrl))
                {
                    urls.Add(new WikiPageModel(Tools.GetPageTitleFromUrl(hrefValue[hrefValue.IndexOf(baseUrl, StringComparison.Ordinal)..]), apiUrl,
                        wikiClientCache));
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
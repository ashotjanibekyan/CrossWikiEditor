namespace CrossWikiEditor.Core.ListProviders;

public sealed class HtmlScraperListProvider(HtmlAgilityPackParser htmlAgilityPackParser, IHttpClientFactory httpClientFactory, ILogger logger,
    SimpleHtmlParser simpleHtmlParser) : UnlimitedListProviderBase
{
    public override string Title => "HTML Scraper";
    public override string ParamTitle => "URL";
    
    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            using HttpClient httpClient = httpClientFactory.CreateClient("Scraper");
            string html = await httpClient.GetStringAsync(Param);
            var urls = new List<WikiPageModel>();
            urls.AddRange(await simpleHtmlParser.GetPages(html));
            urls.AddRange(htmlAgilityPackParser.GetPages(html));
            return urls.Distinct().ToList();
        }
        catch (Exception e)
        {
            logger.Fatal(e, $"{nameof(HtmlScraperListProvider)} failed to make a list");
            return e;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services.HtmlParsers;
using CrossWikiEditor.Core.Utils;
using Serilog;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class HtmlScraperListProvider : UnlimitedListProviderBase
{
    private readonly HtmlAgilityPackParser _htmlAgilityPackParser;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly SimpleHtmlParser _simpleHtmlParser;

    public HtmlScraperListProvider(HtmlAgilityPackParser htmlAgilityPackParser,
        IHttpClientFactory httpClientFactory,
        ILogger logger,
        SimpleHtmlParser simpleHtmlParser)
    {
        _htmlAgilityPackParser = htmlAgilityPackParser;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _simpleHtmlParser = simpleHtmlParser;
    }

    public override string Title => "HTML Scraper";
    public override string ParamTitle => "URL";

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            using HttpClient httpClient = _httpClientFactory.CreateClient("Scraper");
            string html = await httpClient.GetStringAsync(Param);
            var urls = new List<WikiPageModel>();
            urls.AddRange(await _simpleHtmlParser.GetPages(html));
            urls.AddRange(_htmlAgilityPackParser.GetPages(html));
            return urls.Distinct().ToList();
        }
        catch (Exception e)
        {
            _logger.Fatal(e, $"{nameof(HtmlScraperListProvider)} failed to make a list");
            return e;
        }
    }
}
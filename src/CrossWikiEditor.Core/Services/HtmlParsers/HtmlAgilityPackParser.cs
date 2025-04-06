using System;
using System.Collections.Generic;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using HtmlAgilityPack;
using Serilog;

namespace CrossWikiEditor.Core.Services.HtmlParsers;

public sealed class HtmlAgilityPackParser
{
    private readonly ILogger _logger;
    private readonly ISettingsService _settingsService;
    private readonly IWikiClientCache _wikiClientCache;

    public HtmlAgilityPackParser(ILogger logger, ISettingsService settingsService, IWikiClientCache wikiClientCache)
    {
        _logger = logger;
        _settingsService = settingsService;
        _wikiClientCache = wikiClientCache;
    }

    public List<WikiPageModel> GetPages(string html)
    {
        string baseUrl = _settingsService.GetCurrentSettings().GetBaseUrl();
        string apiUrl = _settingsService.GetCurrentSettings().GetApiUrl();
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
                        _wikiClientCache));
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, $"{nameof(HtmlAgilityPackParser)} failed to parse a URL");
            }
        }

        return urls;
    }
}
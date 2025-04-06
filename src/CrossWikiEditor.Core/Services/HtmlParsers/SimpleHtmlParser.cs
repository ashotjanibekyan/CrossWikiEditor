using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using Serilog;

namespace CrossWikiEditor.Core.Services.HtmlParsers;

public sealed class SimpleHtmlParser
{
    private readonly char[] _terminationChars = [' ', '\t', '\n', '"', '<', '>', '{', '}', '&'];
    private readonly ILogger _logger;
    private readonly ISettingsService _settingsService;
    private readonly IWikiClientCache _wikiClientCache;

    public SimpleHtmlParser(ILogger logger,
        ISettingsService settingsService,
        IWikiClientCache wikiClientCache)
    {
        _logger = logger;
        _settingsService = settingsService;
        _wikiClientCache = wikiClientCache;
    }

    public async Task<List<WikiPageModel>> GetPages(string html)
    {
        string[] results = html.Split(_settingsService.GetCurrentSettings().GetBaseUrl());
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
                _logger.Debug(ex, $"{nameof(SimpleHtmlParser)} failed to parse a URL");
            }
        }

        return wikiPageModels;
    }

    private async Task<WikiPageModel> TryGetWikiPageModel(string urlStart)
    {
        string baseUrl = _settingsService.GetCurrentSettings().GetBaseUrl();
        string apiUrl = _settingsService.GetCurrentSettings().GetApiUrl();
        string url = urlStart;
        int i = urlStart.IndexOfAny(_terminationChars);
        if (i != -1)
        {
            url = urlStart[..i];
        }

        if (url[^1] is '\'' or '\"')
        {
            var page = new WikiPageModel(Tools.GetPageTitleFromUrl(baseUrl + url[..^1]), apiUrl, _wikiClientCache);
            if (await page.Exists())
            {
                return page;
            }

            return new WikiPageModel(Tools.GetPageTitleFromUrl(baseUrl + url), apiUrl, _wikiClientCache);
        }

        return new WikiPageModel(Tools.GetPageTitleFromUrl(baseUrl + url), apiUrl, _wikiClientCache);
    }
}
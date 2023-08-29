using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using HtmlAgilityPack;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.ListProviders;

public class HtmlScraperListProvider(
    IUserPreferencesService userPreferencesService, 
    IWikiClientCache wikiClientCache,
    LanguageSpecificRegexes languageSpecificRegexes) : IListProvider
{
    public string Title => "HTML Scraper";
    public string ParamTitle => "URL";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            string baseUrl = userPreferencesService.GetCurrentPref().UrlBase();
            WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(Param);

            var urls = new List<WikiPageModel>();
            urls.AddRange(TerminationCharsBasedParser(html, baseUrl, site));
            urls.AddRange(HtmlAgilityPackBasedParser(html, baseUrl, site));
            return Result<List<WikiPageModel>>.Success(urls.Distinct().ToList());
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    private static List<WikiPageModel> TerminationCharsBasedParser(string html, string baseUrl, WikiSite site)
    {
        var terminationChars = new[] {' ', '\t', '\n', '"', '<', '>', '{', '}', '&'};
            
        var results = html.Split(baseUrl);
        var wikiPageModels = new List<WikiPageModel>();

        foreach (string urlStart in results)
        {
            if (urlStart == string.Empty)
            {
                continue;
            }
            var url = urlStart;
            var i = urlStart.IndexOfAny(terminationChars);
            if (i != -1)
            {
                url = urlStart[..i];
            }

            try
            {
                WikiPageModel page;
                if (url.Last() == '\'' || url.Last() == '\"')
                {
                    page = new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url[..^1])));
                    wikiPageModels.Add(page.WikiPage is {Exists: true}
                        ? page
                        : new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url))));
                }
                else
                {
                    wikiPageModels.Add(new WikiPageModel(new WikiPage(site, Tools.GetPageTitleFromUrl(baseUrl + url))));
                }
                
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return wikiPageModels;
    }

    private static List<WikiPageModel> HtmlAgilityPackBasedParser(string html, string baseUrl, WikiSite site)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var urls = new List<WikiPageModel>();

        foreach (HtmlNode? link in doc.DocumentNode.SelectNodes("//a[@href]"))
        {
            try
            {
                var hrefValue = link.GetAttributeValue("href", string.Empty);

                if (hrefValue.Contains(baseUrl))
                {
                    urls.Add(new WikiPageModel(new WikiPage(site,
                        Tools.GetPageTitleFromUrl(hrefValue[hrefValue.IndexOf(baseUrl, StringComparison.Ordinal)..]))));
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return urls;
    }
}
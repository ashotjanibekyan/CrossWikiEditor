using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders;

public partial class GoogleSearchListProvider(LanguageSpecificRegexes languageSpecificRegexes,
        IUserPreferencesService userPreferencesService,
        IWikiClientCache wikiClientCache)
    : IListProvider
{
    private static readonly Regex _regexGoogle = RegexGoogle();
    public string Title => "Google search";
    public string ParamTitle => "Google search";
    public string Param { get; set; } = string.Empty;
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            var list = new List<WikiPageModel>();

            int intStart = 0;
            string google = HttpUtility.UrlEncode(Param);

            using var client = new HttpClient();
            do
            {
                string url =
                    $"https://www.google.com/search?q={google}+site:{userPreferencesService.GetCurrentPref().UrlBase()}&num=100&hl=en&lr=&start={intStart}&sa=N&filter=0";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throw an exception if HTTP response is not successful

                string googleText = await response.Content.ReadAsStringAsync();
                //Find each match to the pattern
                foreach (Match m in _regexGoogle.Matches(googleText))
                {
                    string searchres = m.Groups["title"].Value;

                    if (searchres.Contains(@"&amp;"))
                    {
                        searchres = searchres[..searchres.IndexOf(@"&amp;", StringComparison.Ordinal)];
                    }

                    await languageSpecificRegexes.InitAsync;
                    string? title = Tools.GetTitleFromURL(searchres, languageSpecificRegexes.ExtractTitle);

                    // some google results are double encoded, so WikiDecode again
                    if (!string.IsNullOrEmpty(title))
                    {
                        title = Regex.Replace(Tools.WikiDecode(title), @"\?\w+=.*", "");
                        Result<WikiPageModel> result =
                            await wikiClientCache.GetWikiPageModel(userPreferencesService.GetCurrentPref().UrlApi(), title);
                        list.Add(result is { IsSuccessful: true, Value: not null } ? result.Value : new WikiPageModel(title, 0));
                    }
                }

                if (!googleText.Contains("img src=\"nav_next.gif\""))
                {
                    break;
                }

                intStart += 100;
            } while (true);

            return Result<List<WikiPageModel>>.Success(list);
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    [GeneratedRegex("href\\s*=\\s*(?:\"(?:/url\\?q=)?(?<title>[^\"]*)\"|(?<title>\\S+) class=l)", RegexOptions.IgnoreCase | RegexOptions.Compiled,
        "en-US")]
    private static partial Regex RegexGoogle();
}
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

public partial class GoogleSearchListProvider : IListProvider
{
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IWikiClientCache _wikiClientCache;
    private readonly LanguageSpecificRegexes _languageSpecificRegexes;

    public GoogleSearchListProvider(LanguageSpecificRegexes languageSpecificRegexes,
        IUserPreferencesService userPreferencesService,
        IWikiClientCache wikiClientCache)
    {
        _userPreferencesService = userPreferencesService;
        _wikiClientCache = wikiClientCache;
        _languageSpecificRegexes = languageSpecificRegexes;
    }

    private static readonly Regex _regexGoogle = RegexGoogle();
    public string Title => "Google search";
    public string ParamTitle => "Google search";
    public string Param { get; set; }
    public bool CanMake => !string.IsNullOrWhiteSpace(Param);
    public bool NeedsAdditionalParams => false;

    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            if (!_languageSpecificRegexes.IsInitialized)
            {
                await _languageSpecificRegexes.InitAsync;
            }

            var list = new List<WikiPageModel>();

            int intStart = 0;
            string google = HttpUtility.UrlEncode(Param);

            using var client = new HttpClient();
            do
            {
                string url =
                    $"https://www.google.com/search?q={google}+site:{_userPreferencesService.GetCurrentPref().UrlBase()}&num=100&hl=en&lr=&start={intStart}&sa=N&filter=0";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throw an exception if HTTP response is not successful

                string googleText = await response.Content.ReadAsStringAsync();
                //Find each match to the pattern
                foreach (Match m in _regexGoogle.Matches(googleText))
                {
                    string searchres = m.Groups["title"].Value;

                    if (searchres.Contains(@"&amp;"))
                    {
                        searchres = searchres.Substring(0, searchres.IndexOf(@"&amp;", StringComparison.Ordinal));
                    }

                    string? title = Tools.GetTitleFromURL(searchres, _languageSpecificRegexes.ExtractTitle);

                    // some google results are double encoded, so WikiDecode again
                    if (!string.IsNullOrEmpty(title))
                    {
                        title = Regex.Replace(Tools.WikiDecode(title), @"\?\w+=.*", "");
                        Result<WikiPageModel> result =
                            await _wikiClientCache.GetWikiPageModel(_userPreferencesService.GetCurrentPref().UrlApi(), title);
                        list.Add(result is {IsSuccessful: true, Value: not null} ? result.Value : new WikiPageModel(title, 0));
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

    public Task GetAdditionalParams()
    {
        return Task.CompletedTask;
    }

    [GeneratedRegex("href\\s*=\\s*(?:\"(?:/url\\?q=)?(?<title>[^\"]*)\"|(?<title>\\S+) class=l)", RegexOptions.IgnoreCase | RegexOptions.Compiled,
        "en-US")]
    private static partial Regex RegexGoogle();
}
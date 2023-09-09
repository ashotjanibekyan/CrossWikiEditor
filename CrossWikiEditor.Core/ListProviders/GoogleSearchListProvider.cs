using System.Text.RegularExpressions;
using System.Web;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using Tools = CrossWikiEditor.Core.Utils.Tools;

namespace CrossWikiEditor.Core.ListProviders;

public partial class GoogleSearchListProvider(
    IUserPreferencesService userPreferencesService,
    IWikiClientCache wikiClientCache,
    IDialogService dialogService) : LimitedListProviderBase(dialogService)
{
    private static readonly Regex _regexGoogle = RegexGoogle();
    public override string Title => "Google search";
    public override string ParamTitle => "Google search";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
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

                    string? title = Tools.GetPageTitleFromUrl(searchres);

                    // some google results are double encoded, so WikiDecode again
                    if (!string.IsNullOrEmpty(title))
                    {
                        title = Regex.Replace(Tools.WikiDecode(title), @"\?\w+=.*", "");
                        Result<WikiPageModel> result =
                            await wikiClientCache.GetWikiPageModel(userPreferencesService.CurrentApiUrl, title);
                        if (result is {IsSuccessful: true, Value: not null})
                        {
                            list.Add(result.Value);
                            if (list.Count >= limit)
                            {
                                break;
                            }
                        }
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
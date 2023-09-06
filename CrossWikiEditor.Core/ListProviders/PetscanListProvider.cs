using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.ListProviders;

public class PetscanListProvider(IUserPreferencesService userPreferencesService, IWikiClientCache wikiClientCache) : UnlimitedListProviderBase
{
    public override string Title => "Petscan";
    public override string ParamTitle => "PSID";
    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            WikiSite wikiSite = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
            var wikiPageModels = new List<WikiPageModel>();
            if (long.TryParse(Param, out long id))
            {
                var httpClient = new HttpClient();
                HttpResponseMessage result = await httpClient.GetAsync($"https://petscan.wmflabs.org/?psid={id}&format=plain");
                if (result.IsSuccessStatusCode)
                {
                    string content = await result.Content.ReadAsStringAsync();
                    var titles = content.Split('\n').ToList();
                    wikiPageModels.AddRange(titles.Select(title => new WikiPageModel(new WikiPage(wikiSite, title))));
                }
            }
            
            return Result<List<WikiPageModel>>.Success(wikiPageModels);
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }
}
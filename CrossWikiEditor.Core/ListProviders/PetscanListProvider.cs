namespace CrossWikiEditor.Core.ListProviders;

public class PetscanListProvider(IHttpClientFactory httpClientFactory, IUserPreferencesService userPreferencesService, IWikiClientCache wikiClientCache) : UnlimitedListProviderBase
{
    public override string Title => "Petscan";
    public override string ParamTitle => "PSID";
    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            WikiSite wikiSite = await wikiClientCache.GetWikiSite(userPreferencesService.CurrentApiUrl);
            var wikiPageModels = new List<WikiPageModel>();
            if (long.TryParse(Param, out long id))
            {
                using HttpClient httpClient = httpClientFactory.CreateClient("Petscan");
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
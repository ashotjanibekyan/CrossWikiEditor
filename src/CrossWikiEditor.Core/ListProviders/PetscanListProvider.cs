namespace CrossWikiEditor.Core.ListProviders;

public sealed class PetscanListProvider(IHttpClientFactory httpClientFactory, IUserPreferencesService userPreferencesService, IWikiClientCache wikiClientCache) : UnlimitedListProviderBase
{
    public override string Title => "Petscan";
    public override string ParamTitle => "PSID";
    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            var wikiPageModels = new List<WikiPageModel>();
            if (long.TryParse(Param, out long id))
            {
                using HttpClient httpClient = httpClientFactory.CreateClient("Petscan");
                HttpResponseMessage result = await httpClient.GetAsync($"https://petscan.wmflabs.org/?psid={id}&format=plain");
                if (result.IsSuccessStatusCode)
                {
                    string content = await result.Content.ReadAsStringAsync();
                    var titles = content.Split('\n').ToList();
                    wikiPageModels.AddRange(titles.Select(title => new WikiPageModel(title, userPreferencesService.CurrentApiUrl, wikiClientCache)));
                }
                else
                {
                    return new Exception($"Could not get the list. \n{await result.Content.ReadAsStringAsync()}");
                }
            }
            else
            {
                return new Exception($"{Param} is not a valid PSID");
            }
            
            return wikiPageModels;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}
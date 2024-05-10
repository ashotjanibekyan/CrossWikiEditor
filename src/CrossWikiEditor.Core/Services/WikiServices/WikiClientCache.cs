namespace CrossWikiEditor.Core.Services.WikiServices;

public interface IWikiClientCache
{
    WikiClient GetWikiClient(string apiRoot, bool forceNew = false);
    Task<WikiSite> GetWikiSite(string apiRoot, bool forceNew = false);
    Task<Result<WikiPageModel>> GetWikiPageModel(string apiRoot, string title);
}

public sealed class WikiClientCache(ILogger logger) : IWikiClientCache
{
    private ConcurrentDictionary<string, WikiClient> _wikiClients = new();
    private ConcurrentDictionary<string, WikiSite> _wikiSites = new();

    public WikiClient GetWikiClient(string apiRoot, bool forceNew = false)
    {
        if (forceNew || !_wikiClients.TryGetValue(apiRoot, out WikiClient? client))
        {
            client = new WikiClient();
            _wikiClients[apiRoot] = client;
        }
        return client;
    }

    public async Task<WikiSite> GetWikiSite(string apiRoot, bool forceNew = false)
    {
        if (forceNew || !_wikiSites.TryGetValue(apiRoot, out WikiSite? site))
        {
            site = new WikiSite(GetWikiClient(apiRoot, forceNew), apiRoot);
            await site.Initialization;
            _wikiSites[apiRoot] = site;
        }

        return site;
    }

    public async Task<Result<WikiPageModel>> GetWikiPageModel(string apiRoot, string title)
    {
        try
        {
            var page = new WikiPage(await GetWikiSite(apiRoot), title);
            return Result<WikiPageModel>.Success(new WikiPageModel(page));
        }
        catch (Exception e)
        {
            logger.Information(e, "Failed to create WikiPage for {} on {}", title, apiRoot);
            return Result<WikiPageModel>.Failure(e.Message);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public static class WikiClientCache
{
    private static Dictionary<string, WikiClient> _wikiClients = new();
    private static Dictionary<string, WikiSite> _wikiSites = new();

    public static WikiClient GetWikiClient(string apiRoot, bool forceNew = false)
    {
        if (!_wikiClients.ContainsKey(apiRoot) || forceNew)
        {
            _wikiClients[apiRoot] = new WikiClient();
        }

        return _wikiClients[apiRoot];
    }

    public static async Task<WikiSite> GetWikiSite(string apiRoot, bool forceNew = false)
    {
        if (!_wikiSites.ContainsKey(apiRoot) || forceNew)
        {
            var site = new WikiSite(GetWikiClient(apiRoot, forceNew), apiRoot);
            await site.Initialization;
            _wikiSites[apiRoot] = site;
        }

        return _wikiSites[apiRoot];
    }
}
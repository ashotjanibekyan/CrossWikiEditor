using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Utils;
using Serilog;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.Services.WikiServices;

public interface IWikiClientCache
{
    WikiClient GetWikiClient(string apiRoot, bool forceNew = false);
    Task<WikiSite> GetWikiSite(string apiRoot, bool forceNew = false);
    Task<Result<WikiPageModel>> GetWikiPageModel(string apiRoot, string title);
}

public sealed class WikiClientCache : IWikiClientCache
{
    // Wikimedia rejects requests without a descriptive User-Agent (HTTP 403).
    // See https://meta.wikimedia.org/wiki/User-Agent_policy
    public const string UserAgent = "CrossWikiEditor/1.0 (https://github.com/ashotjanibekyan/CrossWikiEditor)";

    private readonly ConcurrentDictionary<string, WikiClient> _wikiClients = new();
    private readonly ConcurrentDictionary<string, WikiSite> _wikiSites = new();
    private readonly ILogger _logger;

    public WikiClientCache(ILogger logger)
    {
        _logger = logger;
    }

    public WikiClient GetWikiClient(string apiRoot, bool forceNew = false)
    {
        if (forceNew || !_wikiClients.TryGetValue(apiRoot, out WikiClient? client))
        {
            client = CreateWikiClient();
            _wikiClients[apiRoot] = client;
        }

        return client;
    }

    public static WikiClient CreateWikiClient() => new() { ClientUserAgent = UserAgent };

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
            return new WikiPageModel(page);
        }
        catch (Exception e)
        {
            _logger.Information(e, "Failed to create WikiPage for {} on {}", title, apiRoot);
            return e;
        }
    }
}

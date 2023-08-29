using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using Serilog;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public interface IWikiClientCache
{
    WikiClient GetWikiClient(string apiRoot, bool forceNew = false);
    Task<WikiSite> GetWikiSite(string apiRoot, bool forceNew = false);
    Task<Result<WikiPageModel>> GetWikiPageModel(string apiRoot, string title, bool forceNew = false);
}

public class WikiClientCache(ILogger logger) : IWikiClientCache
{
    private Dictionary<string, WikiClient> _wikiClients = new();
    private Dictionary<string, WikiSite> _wikiSites = new();

    public WikiClient GetWikiClient(string apiRoot, bool forceNew = false)
    {
        if (!_wikiClients.ContainsKey(apiRoot) || forceNew)
        {
            _wikiClients[apiRoot] = new WikiClient();
        }

        return _wikiClients[apiRoot];
    }

    public async Task<WikiSite> GetWikiSite(string apiRoot, bool forceNew = false)
    {
        if (!_wikiSites.ContainsKey(apiRoot) || forceNew)
        {
            var site = new WikiSite(GetWikiClient(apiRoot, forceNew), apiRoot);
            await site.Initialization;
            _wikiSites[apiRoot] = site;
        }

        return _wikiSites[apiRoot];
    }

    public async Task<Result<WikiPageModel>> GetWikiPageModel(string apiRoot, string title, bool forceNew = false)
    {
        try
        {
            var page = new WikiPage(await GetWikiSite(apiRoot, forceNew), title);
            return Result<WikiPageModel>.Success(new WikiPageModel(page));
        }
        catch (Exception e)
        {
            logger.Information(e, "Failed to create WikiPage for {} on {}", title, apiRoot);
            return Result<WikiPageModel>.Failure(e.Message);
        }
    }
}
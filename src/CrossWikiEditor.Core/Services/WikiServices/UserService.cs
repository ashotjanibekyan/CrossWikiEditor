using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;
using Serilog;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.Services.WikiServices;

public interface IUserService
{
    Task<Result<Unit>> Login(Profile profile, string apiRoot);
    Task<Result<List<WikiPageModel>>> GetAllUsers(string apiRoot, string startFrom, int limit);
    Task<Result<List<WikiPageModel>>> GetWatchlistPages(int limit);
    Task<Result<List<WikiPageModel>>> GetUserContributionsPages(string apiRoot, string username, int limit);
}

public sealed class UserService : IUserService
{
    private readonly IWikiClientCache _wikiClientCache;
    private readonly ISettingsService _settingsService;
    private readonly ILogger _logger;

    public UserService(IWikiClientCache wikiClientCache, ISettingsService settingsService, ILogger logger)
    {
        _wikiClientCache = wikiClientCache;
        _settingsService = settingsService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Login(Profile profile, string apiRoot)
    {
        try
        {
            WikiSite site = await _wikiClientCache.GetWikiSite(apiRoot, true);
            await site.LoginAsync(profile.Username, profile.Password);
            return Unit.Default;
        }
        catch (Exception e)
        {
            _logger.Information(e, "Failed to login");
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetAllUsers(string apiRoot, string startFrom, int limit)
    {
        try
        {
            var gen = new AllUsersPageGenerator(await _wikiClientCache.GetWikiSite(apiRoot))
            {
                StartFrom = startFrom
            };
            List<WikiPage> result = await gen.EnumItemsAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Failed to get users. Api: {Api}, startFrom: {StartFrom}, limit: {Limit}", apiRoot, startFrom, limit);
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetWatchlistPages(int limit)
    {
        try
        {
            WikiSite site = await _wikiClientCache.GetWikiSite(_settingsService.CurrentApiUrl);
            var gen = new MyWatchlistGenerator(site);
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return result.ConvertAll(x => new WikiPageModel(x));
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Failed to get watchlist pages");
            return e;
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetUserContributionsPages(string apiRoot, string username, int limit)
    {
        try
        {
            WikiSite site = await _wikiClientCache.GetWikiSite(apiRoot);
            var gen = new UserContributionsGenerator(site, new List<string> {username})
            {
                IncludeTitle = true,
                IncludeIds = true
            };
            List<UserContributionResultItem> result = await gen.EnumItemsAsync().Take(limit).ToListAsync();
            return result.ConvertAll(item => new WikiPageModel(item.WikiPage));
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Failed to get user contrib pages");
            return e;
        }
    }
}
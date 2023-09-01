using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Utils;
using CrossWikiEditor.WikiClientLibraryUtils.Generators;
using Serilog;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public interface IUserService
{
    Task<Result> Login(Profile profile, string apiRoot);
    Task<Result<List<WikiPageModel>>> GetWatchlistPages(int limit);
    Task<Result<List<WikiPageModel>>> GetUserContribsPages(string apiRoot, string username, int limit);
}

public sealed class UserService(IWikiClientCache wikiClientCache, IUserPreferencesService userPreferencesService, ILogger logger)
    : IUserService
{
    public async Task<Result> Login(Profile profile, string apiRoot)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot, true);
            await site.LoginAsync(profile.Username, profile.Password);
            return Result.Success();
        }
        catch (Exception e)
        {
            logger.Information(e, "Failed to login");
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetWatchlistPages(int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
            var gen = new MyWatchlistGenerator(site);
            List<WikiPage> result = await gen.EnumPagesAsync().Take(limit).ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get watchlist pages");
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetUserContribsPages(string apiRoot, string username, int limit)
    {
        try
        {
            // TODO: the library doesn't provide an easy way to retrieve this information.
            // fix the library to implement this part without it
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get user contrib pages");
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }
}
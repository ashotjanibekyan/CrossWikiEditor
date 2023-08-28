using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.WikiClientLibraryUtils.Generators;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public interface IUserService
{
    Task<Result> Login(Profile profile, string apiRoot);
    Task<Result<List<WikiPageModel>>> GetWatchlistPages();
}

public sealed class UserService : IUserService
{
    private readonly IWikiClientCache _wikiClientCache;
    private readonly IUserPreferencesService _userPreferencesService;

    public UserService(IWikiClientCache wikiClientCache, IUserPreferencesService userPreferencesService)
    {
        _wikiClientCache = wikiClientCache;
        _userPreferencesService = userPreferencesService;
    }
    public async Task<Result> Login(Profile profile, string apiRoot)
    {
        try
        {
            WikiSite site = await _wikiClientCache.GetWikiSite(apiRoot, true);
            await site.LoginAsync(profile.Username, profile.Password);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetWatchlistPages()
    {
        try
        {
            WikiSite site = await _wikiClientCache.GetWikiSite(_userPreferencesService.GetCurrentPref().UrlApi());
            if (site.AccountInfo is null)
            {
                return Result<List<WikiPageModel>>.Failure("Please log in to get your watchlist pages.");
            }

            var gen = new MyWatchlistGenerator(site);
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }
}
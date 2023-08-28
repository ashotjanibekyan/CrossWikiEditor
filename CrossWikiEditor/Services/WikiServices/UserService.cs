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
    Task<Result<List<WikiPageModel>>> GetUserContribsPages(string apiRoot, string username);
}

public sealed class UserService(IWikiClientCache wikiClientCache, IUserPreferencesService userPreferencesService)
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
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetWatchlistPages()
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
            var gen = new MyWatchlistGenerator(site);
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetUserContribsPages(string apiRoot, string username)
    {
        try
        {
            // TODO: the library doesn't provide an easy way to retrieve this information.
            // fix the library to implement this part without it
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }
}
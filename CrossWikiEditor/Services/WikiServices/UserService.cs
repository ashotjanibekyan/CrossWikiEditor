using System;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public interface IUserService
{
    Task<Result> Login(Profile profile, string apiRoot);
}

public sealed class UserService : IUserService
{
    private readonly IWikiClientCache _wikiClientCache;

    public UserService(IWikiClientCache wikiClientCache)
    {
        _wikiClientCache = wikiClientCache;
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
}
using System;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public interface IUserService
{
    Task<Result> Login(Profile profile, string apiRoot);
}

public sealed class UserService : IUserService
{
    public async Task<Result> Login(Profile profile, string apiRoot)
    {
        try
        {
            WikiSite site = await WikiClientCache.GetWikiSite(apiRoot, true);
            await site.LoginAsync(profile.Username, profile.Password);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }
}
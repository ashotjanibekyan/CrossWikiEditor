namespace CrossWikiEditor.Core.Services.WikiServices;

public interface IUserService
{
    Task<Result> Login(Profile profile, string apiRoot);
    Task<Result<List<WikiPageModel>>> GetAllUsers(string apiRoot, string startFrom, int limit);
    Task<Result<List<WikiPageModel>>> GetWatchlistPages(int limit);
    Task<Result<List<WikiPageModel>>> GetUserContributionsPages(string apiRoot, string username, int limit);
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

    public async Task<Result<List<WikiPageModel>>> GetAllUsers(string apiRoot, string startFrom, int limit)
    {
        try
        {
            var gen = new AllUsersPageGenerator(await wikiClientCache.GetWikiSite(apiRoot))
            {
                StartFrom = startFrom
            };
            List<WikiPage> result = await gen.EnumItemsAsync().Take(limit).ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get users. Api: {Api}, startFrom: {StartFrom}, limit: {Limit}", apiRoot, startFrom, limit);
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetWatchlistPages(int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.CurrentApiUrl);
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

    public async Task<Result<List<WikiPageModel>>> GetUserContributionsPages(string apiRoot, string username, int limit)
    {
        try
        {
            WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
            var gen = new UserContributionsGenerator(site, new List<string> {username})
            {
                IncludeTitle = true,
                IncludeIds = true
            };
            List<UserContributionResultItem> result = await gen.EnumItemsAsync().Take(limit).ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(item => new WikiPageModel(item.WikiPage)).ToList());
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Failed to get user contrib pages");
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }
}
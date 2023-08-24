using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using CrossWikiEditor.Models;
using WikiClient;
using WikiClient.Actions;

namespace CrossWikiEditor.Services.WikiServices;

public interface IUserService
{
    Task<Result> Login(Profile profile, Site site);
}

public sealed class UserService : IUserService
{
    private ApiQuery _apiQuery;

    private async Task<Result<string>> GetLoginToken(Site site)
    {
        var queryBuilder = new QueryBuilder();

        string? query = queryBuilder
            .WithAction(new QueryActionBuilder()
                .WithMeta(MetaTokenType.login)
                .Build())
            .WithFormat(OutputFormat.Json)
            .Build();

        try
        {
            _apiQuery = new ApiQuery(site);
            RootResponse? result = await _apiQuery.ExecuteGet(query);
            if (result is {BatchComplete: true, Query: not null} && result.Query.ContainsKey("tokens") && result.Query["tokens"] is Tokens tokens)
            {
                return Result<string>.Success(tokens.LoginToken);
            }

            return Result<string>.Failure("The request wasn't successful");
        }
        catch (Exception e)
        {
            return Result<string>.Failure(e.Message);
        }
    }

    private async Task<Result> LoginWithToken(Profile profile, Site site, string token)
    {
        var queryParams = new Dictionary<string, string>
        {
            {"action", "login"},
            {"lgname", profile.Username},
            {"lgpassword", profile.Password},
            {"lgtoken", token},
            {"format", "json"}
        };

        RootResponse? result = await _apiQuery.ExecutePost(ToQueryString(queryParams));
        return Result.Success();
    }

    private static string ToQueryString(Dictionary<string, string> dict)
    {
        NameValueCollection queryParams = HttpUtility.ParseQueryString(string.Empty);
        foreach (KeyValuePair<string, string> kvp in dict)
        {
            queryParams[kvp.Key] = kvp.Value;
        }

        return queryParams.ToString();
    }

    public async Task<Result> Login(Profile profile, Site site)
    {
        Result<string> token = await GetLoginToken(site);
        if (token is {IsSuccessful: true, Value: not null})
        {
            Result result = await LoginWithToken(profile, site, token.Value);
            return result is {IsSuccessful: true} ? Result.Success() : Result.Failure(result.Error);
        }

        return Result.Failure("Could not get login token");
    }
}
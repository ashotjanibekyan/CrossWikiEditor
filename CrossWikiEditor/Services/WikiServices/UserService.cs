﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using CrossWikiEditor.Models;
using WikiClient;
using WikiClient.Actions;

namespace CrossWikiEditor.Services.WikiServices;

public interface IUserService
{
    Task<Result<string>> GetLoginToken(Site site);
    Task<Result<bool>> LoginWithToken(Profile profile, Site site, string token);
}

public sealed class UserService : IUserService
{
    private ApiQuery _apiQuery;
    
    public async Task<Result<string>> GetLoginToken(Site site)
    {
        var queryBuilder = new QueryBuilder();

        var query = queryBuilder
            .WithAction(new QueryActionBuilder()
                .WithMeta(MetaTokenType.login)
                .Build())
            .WithFormat(OutputFormat.Json)
            .Build();

        try
        {
            _apiQuery = new ApiQuery(site);
            var result = await _apiQuery.ExecuteGet(query);
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

    public async Task<Result<bool>> LoginWithToken(Profile profile, Site site, string token)
    {
        var queryParams = new Dictionary<string, string>
        {
            {"action", "login"},
            {"lgname", profile.Username},
            {"lgpassword", profile.Password},
            {"lgtoken", token},
            {"format", "json"}
        };

        var result = await _apiQuery.ExecutePost(ToQueryString(queryParams));
        return Result<bool>.Success(true);
    }
    
    static string ToQueryString(Dictionary<string, string> dict)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        foreach (var kvp in dict) queryParams[kvp.Key] = kvp.Value;
        return queryParams.ToString();
    }
}
using System;
using System.Threading.Tasks;
using WikiClient;
using WikiClient.Actions;

namespace CrossWikiEditor.Services.WikiServices;

public interface IUserService
{
    Task<Result<string>> GetLoginToken(Site site);
    
}

public sealed class UserService : IUserService
{
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
            var apiQuery = new ApiQuery(site);
            var result = await apiQuery.ExecuteGet(query);
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
}
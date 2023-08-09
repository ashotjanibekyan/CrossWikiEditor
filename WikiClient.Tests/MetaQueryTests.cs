using WikiClient.Actions;

namespace WikiClient.Tests;

public class Tests
{
    [Test]
    public async Task GetLoginToken()
    {
        var site = new Site("https://hy.wikipedia.org/w/api.php?");

        var queryBuilder = new QueryBuilder();

        var query = queryBuilder
            .WithAction(new QueryActionBuilder()
                                .WithMeta(MetaTokenType.login)
                                .Build())
            .WithFormat(OutputFormat.Json)
            .Build();
            
            
        var apiQuery = new ApiQuery(site, query);

        var result = await apiQuery.Execute();
        Assert.Multiple(() =>
        {
            Assert.That(result.BatchComplete, Is.True);
            Assert.That(result.Continue, Is.Null);
            Assert.That(result.Query, Is.Not.Null);
            if (result.Query == null) return;
            Assert.That(result.Query.ContainsKey("tokens"));
            Assert.That(result.Query["tokens"], Is.InstanceOf(typeof(Tokens)));
            Assert.That(((Tokens)result.Query["tokens"]).LoginToken, Is.Not.Null);
        });
    }
}
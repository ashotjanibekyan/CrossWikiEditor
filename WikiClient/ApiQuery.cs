using System.Text.Json;
using System.Text.Json.Serialization;

namespace WikiClient;

public class ApiQuery
{
    private readonly Site _site;
    private readonly string _query;

    public ApiQuery(Site site, string query)
    {
        _site = site;
        _query = query;
    }

    public async Task<RootResponse> Execute()
    {
        var httpClient = new HttpClient();

        var url = _site.Domain + _query;
        var response = await httpClient.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();
        var root = JsonSerializer.Deserialize<RootResponse>(responseBody);

        if (root?.Query is not null && root.Query.ContainsKey("tokens"))
        {
            JsonElement jsonElement = (JsonElement) root.Query["tokens"];
            root.Query["tokens"] = jsonElement.Deserialize<Tokens>(options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy()
            }) ?? root.Query["tokens"];
        }
        
        return root;
    }
}
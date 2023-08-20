using System.Text.Json;
using System.Text.Json.Serialization;

namespace WikiClient;

public sealed class ApiQuery
{
    private readonly Site _site;
    private static HttpClient _httpClient = new();

    public ApiQuery(Site site)
    {
        _site = site;
    }

    public async Task<RootResponse> ExecuteGet(string query)
    {
        var url = _site.Domain + query;
        var response = await _httpClient.GetAsync(url);
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

    public async Task<RootResponse> ExecutePost(string query)
    {
        var url = _site.Domain;
        var response = await _httpClient.PostAsync(url, new StringContent(query, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded"));
        var responseBody = await response.Content.ReadAsStringAsync();
        var root = JsonSerializer.Deserialize<RootResponse>(responseBody);
        return root;
    }
}
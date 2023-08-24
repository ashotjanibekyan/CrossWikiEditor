using System.Text.Json;

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
        string? url = _site.Domain + query;
        HttpResponseMessage? response = await _httpClient.GetAsync(url);
        string? responseBody = await response.Content.ReadAsStringAsync();
        RootResponse? root = JsonSerializer.Deserialize<RootResponse>(responseBody);

        if (root?.Query is not null && root.Query.ContainsKey("tokens"))
        {
            var jsonElement = (JsonElement) root.Query["tokens"];
            root.Query["tokens"] = jsonElement.Deserialize<Tokens>(new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy()
            }) ?? root.Query["tokens"];
        }

        return root;
    }

    public async Task<RootResponse> ExecutePost(string query)
    {
        string? url = _site.Domain;
        HttpResponseMessage? response =
            await _httpClient.PostAsync(url, new StringContent(query, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded"));
        string? responseBody = await response.Content.ReadAsStringAsync();
        RootResponse? root = JsonSerializer.Deserialize<RootResponse>(responseBody);
        return root;
    }
}
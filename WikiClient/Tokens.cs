using System.Text.Json.Serialization;

namespace WikiClient;

public sealed class Tokens
{
    [JsonPropertyName("createaccounttoken")]
    public string CreateAccountToken { get; set; }

    [JsonPropertyName("csrftoken")] public string CsrfToken { get; set; }

    [JsonPropertyName("deleteglobalaccounttoken")]
    public string DeleteGlobalAccountToken { get; set; }

    public string LoginToken { get; set; }

    [JsonPropertyName("patroltoken")] public string PatrolToken { get; set; }

    [JsonPropertyName("rollbacktoken")] public string RollbackToken { get; set; }

    [JsonPropertyName("setglobalaccountstatustoken")]
    public string SetGlobalAccountStatusToken { get; set; }

    [JsonPropertyName("userrightstoken")] public string UserRightsToken { get; set; }

    [JsonPropertyName("watchtoken")] public string WatchToken { get; set; }
}
using System.Text.Json.Serialization;

namespace WikiClient;

public class RootResponse
{
    [JsonPropertyName("batchcomplete")]
    public bool BatchComplete { get; set; }
    
    [JsonPropertyName("query")]
    public Dictionary<string, object>? Query { get; set; }
    
    [JsonPropertyName("continue")]
    public Dictionary<string, object>? Continue { get; set; }
}
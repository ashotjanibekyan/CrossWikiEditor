using System.Text.Json.Serialization;
using CrossWikiEditor.Core.JsonConverters;

namespace CrossWikiEditor.Core.Settings;

public sealed class Settings
{
    [JsonPropertyName("normalFindAndReplaceRules")] 
    [JsonConverter(typeof(NormalFindAndReplaceRulesConverter))]
    public NormalFindAndReplaceRules NormalFindAndReplaceRules { get; set; } = [];
}
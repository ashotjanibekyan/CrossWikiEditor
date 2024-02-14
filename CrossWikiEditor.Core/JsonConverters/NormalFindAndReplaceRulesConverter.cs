using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrossWikiEditor.Core.JsonConverters;

public class NormalFindAndReplaceRulesConverter : JsonConverter<NormalFindAndReplaceRules>
{
    public override NormalFindAndReplaceRules? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        JsonProperty? jsonObjectInfo = doc.RootElement.EnumerateObject().FirstOrDefault();
        if (jsonObjectInfo is null)
        {
            return null;
        }
        var result = new NormalFindAndReplaceRules();

        foreach (JsonProperty jsonProperty in doc.RootElement.EnumerateObject())
        {
            switch (jsonProperty.Name)
            {
                case "IgnoreLinks":
                    result.IgnoreLinks = jsonProperty.Value.GetBoolean();
                    break;
                case "IgnoreMore":
                    result.IgnoreMore = jsonProperty.Value.GetBoolean();
                    break;
                case "AddToSummary":
                    result.AddToSummary = jsonProperty.Value.GetBoolean();
                    break;
                case "Rules":
                    List<NormalFindAndReplaceRule>? x = JsonSerializer.Deserialize<List<NormalFindAndReplaceRule>>(jsonProperty.Value.GetRawText(), options);
                    if (x is not null)
                    {
                        result.AddRange(x);
                    }
                    break;
            }
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, NormalFindAndReplaceRules value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteBoolean("IgnoreLinks", value.IgnoreLinks);
        writer.WriteBoolean("IgnoreMore", value.IgnoreMore);
        writer.WriteBoolean("AddToSummary", value.AddToSummary);
        writer.WriteStartArray("Rules");
        foreach (NormalFindAndReplaceRule rule in value)
        {
            JsonSerializer.Serialize(writer, rule, options);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
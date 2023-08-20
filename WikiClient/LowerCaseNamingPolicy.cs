using System.Text.Json;

namespace WikiClient;

public sealed class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) =>
        name.ToLower();
}
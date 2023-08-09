using System.Text.Json;

namespace WikiClient;

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) =>
        name.ToLower();
}
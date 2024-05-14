using Newtonsoft.Json.Linq;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class ExternalUrlUsageGenerator(WikiSite site) : WikiList<ExternalUrlUsageItem>(site)
{
    public override string ListName => "exturlusage";
    public IEnumerable<int>? NamespaceIds { get; set; }
    public string? Url { get; set; }
    public string? Protocol { get; set; }

    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        return new Dictionary<string, object?>()
        {
            {"euprop", "ids|title|url"},
            {"euprotocol", Protocol},
            {"euquery", HttpUtility.UrlEncode(Url)},
            {"eunamespace", NamespaceIds == null ? null : MediaWikiHelper.JoinValues(NamespaceIds)},
            {"eulimit", PaginationSize}
        };
    }

    protected override ExternalUrlUsageItem ItemFromJson(JToken json)
    {
        var jsonObj = (JObject) json;
        return new ExternalUrlUsageItem()
        {
            PageId = (int) jsonObj["pageid"],
            NamespaceId = (int) jsonObj["ns"],
            Title = (string) jsonObj["title"],
            Url = (string) jsonObj["url"]
        };
    }
}
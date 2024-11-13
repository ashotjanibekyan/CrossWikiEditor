using Newtonsoft.Json.Linq;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class AllUsersPageGenerator(WikiSite site) : WikiList<WikiPage>(site)
{
    public string? StartFrom { get; set; } = null;
    public override string ListName => "allusers";

    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        return new Dictionary<string, object?>
        {
            {"aulimit", "max"},
            {"aufrom", StartFrom}
        };
    }

    protected override WikiPage ItemFromJson(JToken json)
    {
        string? name = json["name"]!.Value<string>();
        var wikiPage = new WikiPage(Site, $"User:{name}", 2);
        wikiPage.RefreshAsync();
        return wikiPage;
    }
}
using Newtonsoft.Json.Linq;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class AllUsersPageGenerator : WikiList<WikiPage>
{
    public AllUsersPageGenerator(WikiSite site) : base(site)
    {
    }

    public string? StartFrom { get; set; } = null;
    public override string ListName => "allusers";
    
    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        return new Dictionary<string, object?>()
        {
            {"aulimit", "max"},
            {"aufrom", StartFrom}
        };
    }

    protected override WikiPage ItemFromJson(JToken json)
    {
        var wikiPage = new WikiPage(Site, $"User:{(string) json["name"]}", 2);
        wikiPage.RefreshAsync();
        return wikiPage;
    }

}
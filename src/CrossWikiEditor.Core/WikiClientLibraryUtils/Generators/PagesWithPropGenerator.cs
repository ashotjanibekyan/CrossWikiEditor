namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class PagesWithPropGenerator(WikiSite site, string propertyName) : WikiPageGenerator(site)
{
    /// <summary>
    /// Page property for which to enumerate pages (action=query&list=pagepropnames returns page property names in use).
    /// </summary>
    public string PropertyName { get; set; } = propertyName;

    /// <summary>
    /// Gets/sets a value that indicates whether the links should be listed in
    /// the descending order. (MediaWiki 1.19+)
    /// </summary>
    public bool OrderDescending { get; set; }

    public override string ListName => "pageswithprop";

    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        return new Dictionary<string, object?>
        {
            {"pwpprop", "ids|title|value"},
            {"pwppropname", PropertyName},
            {"pwplimit", PaginationSize},
            {"pwpdir", OrderDescending ? "descending" : "ascending"},
        };
    }
}

namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public class AllPagesGeneratorEx : AllPagesGenerator
{
    public AllPagesGeneratorEx(WikiSite site) : base(site)
    {
    }
    
    public string? ProtectionType { get; set; }
    public string? ProtectionLevel { get; set; }
    
    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        return new Dictionary<string, object?>
        {
            {"apfrom", StartTitle},
            {"apto", EndTitle},
            {"aplimit", PaginationSize},
            {"apnamespace", NamespaceId},
            {"apprefix", Prefix},
            {"apfilterredir", RedirectsFilter.ToString("redirects", "nonredirects")},
            {"apfilterlanglinks", LanguageLinkFilter.ToString("withlanglinks", "withoutlanglinks")},
            {"apminsize", MinPageContentLength},
            {"apmaxsize", MaxPageContentLength},
            {"apprtype", ProtectionType},
            {"apprlevel", ProtectionLevel},
        };
    }
}
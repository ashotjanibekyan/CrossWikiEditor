using System.Collections.Generic;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class AllPagesGeneratorEx : AllPagesGenerator
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
            {"apprlevel", ProtectionLevel}
        };
    }
}
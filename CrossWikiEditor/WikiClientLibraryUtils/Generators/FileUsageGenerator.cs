using System.Collections.Generic;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Generators.Primitive;
using WikiClientLibrary.Infrastructures;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.WikiClientLibraryUtils.Generators;

/// <summary>
/// This class is mostly copied from WikiClientLibrary itself. Once this class is released and is available via Nuget,
/// We should get rid of this class.
/// </summary>
public class FileUsageGenerator : WikiPageGenerator
{
    public FileUsageGenerator(WikiSite site) : base(site)
    {
    }

    public FileUsageGenerator(WikiSite site, string targetTitle) : base(site)
    {
        TargetTitle = targetTitle;
    }

    public string TargetTitle { get; set; } = "";
    public IEnumerable<int>? NamespaceIds { get; set; }
    public PropertyFilterOption RedirectsFilter { get; set; }

    public override string ListName => "imageusage";

    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        return new Dictionary<string, object?>
        {
            {"iutitle", TargetTitle},
            {"iunamespace", NamespaceIds == null ? null : MediaWikiHelper.JoinValues(NamespaceIds)},
            {"iufilterredir", RedirectsFilter.ToString("redirects", "nonredirects")},
            {"iulimit", PaginationSize}
        };
    }
}
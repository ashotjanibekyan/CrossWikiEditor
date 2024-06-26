﻿namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class MyWatchlistGenerator(WikiSite site) : WikiPageGenerator(site)
{
    /// <summary>
    /// Only list pages in the given namespaces.
    /// </summary>
    public IEnumerable<int>? NamespaceIds { get; set; }

    /// <summary>
    /// Adds timestamp of when the user was last notified about the edit.
    /// </summary>
    public bool ShowChangedTime { get; set; }

    /// <summary>
    /// Only show pages that have not been changed.
    /// </summary>
    public PropertyFilterOption NotChangedPagesFilter { get; set; }

    /// <summary>
    /// Gets/sets a value that indicates whether the links should be listed in
    /// the descending order. (MediaWiki 1.19+)
    /// </summary>
    public bool OrderDescending { get; set; }

    /// <summary>
    /// Title (with namespace prefix) to begin enumerating from.
    /// </summary>
    public string? FromTitle { get; set; }

    /// <summary>
    /// Title (with namespace prefix) to stop enumerating at.
    /// </summary>
    public string? ToTitle { get; set; }

    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        return new Dictionary<string, object?>
        {
            {"wrnamespace", NamespaceIds == null ? null : MediaWikiHelper.JoinValues(NamespaceIds)},
            {"wrlimit", PaginationSize},
            {"wrprop", ShowChangedTime ? "changed" : null},
            {"wrshow", NotChangedPagesFilter.ToString("!changed", "changed", null)},
            {"wrdir", OrderDescending ? "descending" : "ascending"},
            {"wrfromtitle", FromTitle},
            {"wrtotitle", ToTitle}
        };
    }

    public override string ListName => "watchlistraw";
}
using System.Collections.Generic;

namespace CrossWikiEditor.Core.Settings;

public sealed class NormalFindAndReplaceRules : List<NormalFindAndReplaceRule>
{
    public NormalFindAndReplaceRules()
    {
    }

    public NormalFindAndReplaceRules(IEnumerable<NormalFindAndReplaceRule> collection) : base(collection)
    {
    }

    public bool IgnoreLinks { get; set; }

    public bool IgnoreMore { get; set; }

    public bool AddToSummary { get; set; }
}
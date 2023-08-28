using System.Collections.Generic;

namespace CrossWikiEditor.Settings;

public class NormalFindAndReplaceRules : List<NormalFindAndReplaceRule>
{
    public NormalFindAndReplaceRules()
    {
    }
    
    public NormalFindAndReplaceRules(IEnumerable<NormalFindAndReplaceRule> collection) : base(collection)
    {
    }
    
    /// <summary>
    /// Ignore external/interwiki links, images, nowiki, math and &lt;!-- --&gt;
    /// </summary>
    public bool IgnoreLinks { get; set; }
    
    /// <summary>
    /// Ignore templates, refs, link targets, and headings
    /// </summary>
    public bool IgnoreMore { get; set; }
    public bool AddToSummary { get; set; }
}
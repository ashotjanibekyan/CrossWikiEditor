using System.Collections.Generic;

namespace CrossWikiEditor.Models;

public class DiffRowNode
{
    public List<string> HtmlClasses { get; set; } = new();
    public string Marker { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int ColSpan { get; set; } = 1;
}
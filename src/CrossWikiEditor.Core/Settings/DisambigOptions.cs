namespace CrossWikiEditor.Core.Settings;

public class DisambigOptions
{
    public bool EnableDisambiguation { get; set; }
    public string LinkToDisambiguate { get; set; } = string.Empty;
    public string SkipPageNoDisambiguationsMade { get; set; } = string.Empty;
    public int ContextCharacterCount { get; set; }
}
namespace CrossWikiEditor.Core;

/// <summary>
/// This message is fired the program starts processing the page (local changes, before saving).
/// </summary>
public sealed class PageProcessingMessage(WikiPageModel wikiPageModel)
{
    public WikiPageModel Page { get; } = wikiPageModel;
}

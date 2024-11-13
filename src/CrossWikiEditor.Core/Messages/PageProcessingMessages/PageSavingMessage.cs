namespace CrossWikiEditor.Core;

/// <summary>
///     This message is fired the program starts saving the page.
/// </summary>
public sealed class PageSavingMessage(WikiPageModel wikiPageModel)
{
    public WikiPageModel Page { get; } = wikiPageModel;
}
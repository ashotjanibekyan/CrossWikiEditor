using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Messages.PageProcessingMessages;

/// <summary>
///     This message is fired the program starts processing the page (local changes, before saving).
/// </summary>
public sealed class PageProcessingMessage
{
    /// <summary>
    ///     This message is fired the program starts processing the page (local changes, before saving).
    /// </summary>
    public PageProcessingMessage(WikiPageModel wikiPageModel)
    {
        Page = wikiPageModel;
    }

    public WikiPageModel Page { get; }
}

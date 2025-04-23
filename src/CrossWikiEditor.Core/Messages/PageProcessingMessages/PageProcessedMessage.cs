using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Messages.PageProcessingMessages;

/// <summary>
///     Message is fired when the local processing is finished and the page is ready to be saved.
/// </summary>
public sealed class PageProcessedMessage
{
    /// <summary>
    ///     Message is fired when the local processing is finished and the page is ready to be saved.
    /// </summary>
    public PageProcessedMessage(WikiPageModel wikiPageModel, bool isSuccessful)
    {
        Page = wikiPageModel;
        IsSuccessful = isSuccessful;
    }

    public WikiPageModel Page { get; }
    public bool IsSuccessful { get; }
}

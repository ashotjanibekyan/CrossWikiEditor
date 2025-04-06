using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core;

/// <summary>
///     This message is fired the program starts saving the page.
/// </summary>
public sealed class PageSavingMessage
{
    /// <summary>
    ///     This message is fired the program starts saving the page.
    /// </summary>
    public PageSavingMessage(WikiPageModel wikiPageModel)
    {
        Page = wikiPageModel;
    }

    public WikiPageModel Page { get; }
}
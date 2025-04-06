using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Messages;

public sealed class PageUpdatingMessage
{
    public PageUpdatingMessage(WikiPageModel page, string initialContent, string newContent)
    {
        Page = page;
        InitialContent = initialContent;
        NewContent = newContent;
    }

    public WikiPageModel Page { get; }
    public string InitialContent { get; }
    public string NewContent { get; }
}
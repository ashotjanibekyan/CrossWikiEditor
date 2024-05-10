namespace CrossWikiEditor.Core.Messages;

public sealed class PageUpdatingMessage(WikiPageModel page, string initialContent, string newContent)
{
    public WikiPageModel Page { get; } = page;
    public string InitialContent { get; } = initialContent;
    public string NewContent { get; } = newContent;
}
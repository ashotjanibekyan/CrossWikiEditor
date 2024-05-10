namespace CrossWikiEditor.Core.Messages;

public sealed class PageSkippedMessage(WikiPageModel wikiPageModel, string skipReason)
{
    public WikiPageModel Page { get; } = wikiPageModel;
    public string SkipReason { get; } = skipReason;
}
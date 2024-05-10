namespace CrossWikiEditor.Core.Messages;

public sealed class PageSkippedMessage(WikiPageModel wikiPageModel, SkipReason skipReason)
{
    public WikiPageModel Page { get; } = wikiPageModel;
    public SkipReason SkipReason { get; } = skipReason;
}

public enum SkipReason
{
    Manual,
    ErrorProcessing,
    NoChanges
}
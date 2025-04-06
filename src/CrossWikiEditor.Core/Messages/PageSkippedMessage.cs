using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Messages;

public sealed class PageSkippedMessage
{
    public PageSkippedMessage(WikiPageModel wikiPageModel, SkipReason skipReason)
    {
        Page = wikiPageModel;
        SkipReason = skipReason;
    }

    public WikiPageModel Page { get; }
    public SkipReason SkipReason { get; }
}

public enum SkipReason
{
    Manual,
    ErrorProcessing,
    NoChanges
}
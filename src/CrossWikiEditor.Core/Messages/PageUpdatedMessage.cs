using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Messages;

public sealed class PageUpdatedMessage
{
    public PageUpdatedMessage(WikiPageModel wikiPageModel)
    {
        Page = wikiPageModel;
    }

    public WikiPageModel Page { get; }
}
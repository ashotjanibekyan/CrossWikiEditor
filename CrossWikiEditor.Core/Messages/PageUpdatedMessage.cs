namespace CrossWikiEditor.Core.Messages;

public sealed class PageUpdatedMessage(WikiPageModel wikiPageModel)
{
    public WikiPageModel Page { get; } = wikiPageModel;
}
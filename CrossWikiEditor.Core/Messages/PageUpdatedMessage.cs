using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Messages;

public class PageUpdatedMessage(WikiPageModel wikiPageModel, string oldContent, string newContent)
{
    public WikiPageModel Page { get; } = wikiPageModel;
    public string OldContent { get; } = oldContent;
    public string NewContent { get; } = newContent;
}
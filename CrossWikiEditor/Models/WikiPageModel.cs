using WikiClientLibrary.Pages;

namespace CrossWikiEditor.Models;

public record WikiPageModel(string Title, int NamespaceId = 0)
{
    public WikiPage? WikiPage { get; set; }

    public WikiPageModel(WikiPage wikiPage) : this(wikiPage.Title, wikiPage.NamespaceId)
    {
        WikiPage = wikiPage;
    }
}
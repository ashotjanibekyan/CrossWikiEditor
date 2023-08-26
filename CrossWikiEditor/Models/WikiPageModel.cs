using WikiClientLibrary.Pages;

namespace CrossWikiEditor.Models;

public partial record WikiPageModel(string Title, int NamespaceId)
{
    public WikiPage? WikiPage { get; set; }

    public WikiPageModel(WikiPage wikiPage) : this(wikiPage.Title, wikiPage.NamespaceId)
    {
        WikiPage = wikiPage;
    }
}
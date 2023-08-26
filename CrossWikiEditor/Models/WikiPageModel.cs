using WikiClientLibrary.Pages;

namespace CrossWikiEditor.Models;

public record WikiPageModel(string Title)
{
    public WikiPageModel(WikiPage wikiPage) : this(wikiPage.Title)
    {
    }
}
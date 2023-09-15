namespace CrossWikiEditor.Core.Services.HtmlParsers;

public interface IHtmlParser
{
    Task<List<WikiPageModel>> GetPages(string html);
}
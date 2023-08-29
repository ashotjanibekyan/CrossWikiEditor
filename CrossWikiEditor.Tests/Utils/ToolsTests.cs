using CrossWikiEditor.Utils;

namespace CrossWikiEditor.Tests.Utils;

public class ToolsTests
{

    [TestCase("https://en.wikipedia.org/wiki/Artificial_intelligence", ExpectedResult = "Artificial intelligence")]
    [TestCase("https://en.wikipedia.org/wiki/Mathematics?", ExpectedResult = "Mathematics")]
    [TestCase("https://en.wikipedia.org/wiki/Artificial_intelligence?foo=bar", ExpectedResult = "Artificial intelligence")]
    [TestCase("https://hy.wikipedia.org/wiki/%D5%80%D5%A1%D5%B5%D5%A1%D5%BD%D5%BF%D5%A1%D5%B6", ExpectedResult = "Հայաստան")]
    [TestCase("https://hy.wikipedia.org/wiki/%25D5%2580%25D5%25A1%25D5%25B5%25D5%25A1%25D5%25BD%25D5%25BF%25D5%25A1%25D5%25B6", ExpectedResult = "Հայաստան")]
    [TestCase("https://fringe.fandom.com/wiki/White_Tulip", ExpectedResult = "White Tulip")]
    public string GetPageTitleFromUrl_ShouldParseDirectLinks(string url)
    {
        return Tools.GetPageTitleFromUrl(url);
    }

    [TestCase("https://en.wikipedia.org/w/index.php?title=Artificial_intelligence&action=history", ExpectedResult = "Artificial intelligence")]
    [TestCase("https://en.wikipedia.org/w/index.php?title=Mathematics&action=edit", ExpectedResult = "Mathematics")]
    [TestCase("https://en.wikipedia.org/w/index.php?title=Artificial_intelligence&veaction=edit", ExpectedResult = "Artificial intelligence")]
    [TestCase("https://hy.wikipedia.org/w/index.php?title=%D5%80%D5%A1%D5%B5%D5%A1%D5%BD%D5%BF%D5%A1%D5%B6&action=history", ExpectedResult = "Հայաստան")]
    [TestCase("https://hy.wikipedia.org/w/index.php?title=%D5%80%D5%A1%D5%B5%D5%A1%D5%BD%D5%BF%D5%A1%D5%B6&action=edit", ExpectedResult = "Հայաստան")]
    [TestCase("https://fringe.fandom.com/wiki/White_Tulip?action=history", ExpectedResult = "White Tulip")]
    public string GetPageTitleFromUrl_ShouldParseSecondaryLinks(string url)
    {
        return Tools.GetPageTitleFromUrl(url);
    }
}
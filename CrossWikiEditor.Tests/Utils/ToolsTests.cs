namespace CrossWikiEditor.Tests.Utils;

public sealed class ToolsTests
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
    [TestCase("https://en.wikipedia.org/w/index.php?title=Mathematics", ExpectedResult = "Mathematics")]
    [TestCase("https://en.wikipedia.org/w/index.php?title=Artificial_intelligence&veaction=edit", ExpectedResult = "Artificial intelligence")]
    [TestCase("https://hy.wikipedia.org/w/index.php?title=%D5%80%D5%A1%D5%B5%D5%A1%D5%BD%D5%BF%D5%A1%D5%B6&action=history", ExpectedResult = "Հայաստան")]
    [TestCase("https://hy.wikipedia.org/w/index.php?title=%D5%80%D5%A1%D5%B5%D5%A1%D5%BD%D5%BF%D5%A1%D5%B6&action=edit", ExpectedResult = "Հայաստան")]
    [TestCase("https://fringe.fandom.com/wiki/White_Tulip?action=history", ExpectedResult = "White Tulip")]
    public string GetPageTitleFromUrl_ShouldParseSecondaryLinks(string url)
    {
        return Tools.GetPageTitleFromUrl(url);
    }
    
    [Test]
    public void FirstDifference_ShouldReturnStringLength_WhenStringsAreIdentical()
    {
        // Arrange
        const string a = "abcdef";
        const string b = "abcdef";

        // Act
        int result = Tools.FirstDifference(a, b);

        // Assert
        result.Should().Be(a.Length);
    }

    [Test]
    public void FirstDifference_ShouldReturnZero_WhenStringsDifferAtFirstPosition()
    {
        // Arrange
        const string a = "abcdef";
        const string b = "bbcdef";

        // Act
        int result = Tools.FirstDifference(a, b);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void FirstDifference_ShouldReturnPosition_WhenStringsDifferAtMiddlePosition()
    {
        // Arrange
        const string a = "abcdef";
        const string b = "abzdef";

        // Act
        int result = Tools.FirstDifference(a, b);

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public void FirstDifference_ShouldReturnShorterStringLength_WhenStringAIsShorter()
    {
        // Arrange
        const string a = "abc";
        const string b = "abcdef";

        // Act
        int result = Tools.FirstDifference(a, b);

        // Assert
        result.Should().Be(a.Length);
    }

    [Test]
    public void FirstDifference_ShouldReturnShorterStringLength_WhenStringBIsShorter()
    {
        // Arrange
        const string a = "abcdef";
        const string b = "ab";

        // Act
        int result = Tools.FirstDifference(a, b);

        // Assert
        result.Should().Be(b.Length);
    }
    
        [Test]
    public void RemoveSyntax_ShouldReturnEmptyString_WhenInputIsNull()
    {
        // Arrange
        string input = null;

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().BeNullOrEmpty();
    }

    [TestCase("")]
    [TestCase("   ")]
    public void RemoveSyntax_ShouldReturnEmptyString_WhenInputIsEmpty(string input)
    {
        // Arrange

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().BeNullOrEmpty();
    }

    [Test]
    public void RemoveSyntax_ShouldRemoveLeadingHash_WhenInputStartsWithHash()
    {
        // Arrange
        string input = "#Hashtag";

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().Be("Hashtag");
    }

    [Test]
    public void RemoveSyntax_ShouldRemoveLeadingAsterisk_WhenInputStartsWithAsterisk()
    {
        // Arrange
        string input = "*Asterisk";

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().Be("Asterisk");
    }

    [Test]
    public void RemoveSyntax_ShouldRemoveUnderscoresAndTrimSpaces()
    {
        // Arrange
        string input = " _Remove_ _ Underscores _ ";

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().Be("Remove    Underscores");
    }

    [Test]
    public void RemoveSyntax_ShouldRemoveSquareBrackets()
    {
        // Arrange
        const string input = "[Remove Square Brackets]";

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().Be("Remove Square Brackets");
    }

    [Test]
    public void RemoveSyntax_ShouldReplaceAmpersandEntities()
    {
        // Arrange
        const string input = "Replace &amp; with &";

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().Be("Replace & with &");
    }

    [Test]
    public void RemoveSyntax_ShouldReplaceQuotationEntities()
    {
        // Arrange
        const string input = "Replace &quot; with \"";

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().Be("Replace \" with \"");
    }

    [Test]
    public void RemoveSyntax_ShouldRemoveSpecialCharacter()
    {
        // Arrange
        const string input = "Remove special character �";

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().Be("Remove special character ");
    }

    [Test]
    public void RemoveSyntax_ShouldRemoveLeadingColon()
    {
        // Arrange
        const string input = ":Remove leading colon";

        // Act
        string result = Tools.RemoveSyntax(input);

        // Assert
        result.Should().Be("Remove leading colon");
    }
}
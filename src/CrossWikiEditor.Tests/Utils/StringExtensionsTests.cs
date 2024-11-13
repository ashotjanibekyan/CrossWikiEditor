using System.Runtime.InteropServices;

namespace CrossWikiEditor.Tests.Utils;

public sealed class StringExtensionsTests
{
    [Test]
    public void ToFilenameSafe_ReplacesInvalidCharsWithHyphen()
    {
        // Arrange
        const string input = "file*name?with|invalid/characters";
        const string expectedWindows = "file-name-with-invalid-characters";
        const string expectedUnix = "file*name?with|invalid-characters";

        // Act
        string result = input.ToFilenameSafe();

        // Assert
        result.Should().Be(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? expectedWindows : expectedUnix);
    }

    [Test]
    public void ToFilenameSafe_LeavesValidCharsUnchanged()
    {
        // Arrange
        const string input = "filename-with-valid-chars";
        const string expected = "filename-with-valid-chars";

        // Act
        string result = input.ToFilenameSafe();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void ToFilenameSafe_EmptyString_ReturnsEmptyString()
    {
        // Arrange
        string input = string.Empty;

        // Act
        string result = input.ToFilenameSafe();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Test]
    public void Contains_SubstringFound_ReturnsTrue()
    {
        // Arrange
        const string input = "This is a test string.";
        const string substring = "test";

        // Act
        bool result = input.Contains(substring, false);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Contains_SubstringNotFound_ReturnsFalse()
    {
        // Arrange
        const string input = "This is a test string.";
        const string substring = "missing";

        // Act
        bool result = input.Contains(substring, false);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Contains_RegexMatch_ReturnsTrue()
    {
        // Arrange
        const string input = "123-456-7890";
        const string regexPattern = @"\d{3}-\d{3}-\d{4}";

        // Act
        bool result = input.Contains(regexPattern, true);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Contains_RegexNoMatch_ReturnsFalse()
    {
        // Arrange
        const string input = "This is not a phone number.";
        const string regexPattern = @"\d{3}-\d{3}-\d{4}";

        // Act
        bool result = input.Contains(regexPattern, true);

        // Assert
        result.Should().BeFalse();
    }

    [TestCase("this is the main string", "the main", ExpectedResult = true)]
    [TestCase("this is the main string", "the main f", ExpectedResult = false)]
    [TestCase("this is the main string", "the Main", ExpectedResult = false)]
    public bool Contains_Substring_CaseSensitive(string text, string substring)
    {
        // arrange

        // act
        bool result = text.Contains(substring, false, true);

        // assert
        return result;
    }

    [TestCase("this is the main string", "the main", ExpectedResult = true)]
    [TestCase("this is the main string", "the main f", ExpectedResult = false)]
    [TestCase("this is the main string", "the Main", ExpectedResult = true)]
    public bool Contains_Substring_CaseSensitivity(string text, string substring)
    {
        // arrange

        // act
        bool result = text.Contains(substring, false, false);

        // assert
        return result;
    }
}
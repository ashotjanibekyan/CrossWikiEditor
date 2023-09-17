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
        bool result = input.Contains(substring, isRegex: false);

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
        bool result = input.Contains(substring, isRegex: false);

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
        bool result = input.Contains(regexPattern, isRegex: true);

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
        bool result = input.Contains(regexPattern, isRegex: true);

        // Assert
        result.Should().BeFalse();
    }
}
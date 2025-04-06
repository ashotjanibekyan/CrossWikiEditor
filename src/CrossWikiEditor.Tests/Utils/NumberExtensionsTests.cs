using CrossWikiEditor.Core.Utils.Extensions;

namespace CrossWikiEditor.Tests.Utils;

public sealed class NumberExtensionsTests
{
    [TestCase(0, ExpectedResult = true)]
    [TestCase(-2, ExpectedResult = true)]
    [TestCase(2, ExpectedResult = true)]
    [TestCase(-10, ExpectedResult = true)]
    [TestCase(10, ExpectedResult = true)]
    [TestCase(1, ExpectedResult = false)]
    [TestCase(-1, ExpectedResult = false)]
    [TestCase(3, ExpectedResult = false)]
    [TestCase(-11, ExpectedResult = false)]
    [TestCase(11, ExpectedResult = false)]
    public bool IsEven(int value)
    {
        return value.IsEven();
    }

    [TestCase(0, ExpectedResult = false)]
    [TestCase(-2, ExpectedResult = false)]
    [TestCase(2, ExpectedResult = false)]
    [TestCase(-10, ExpectedResult = false)]
    [TestCase(10, ExpectedResult = false)]
    [TestCase(1, ExpectedResult = true)]
    [TestCase(-1, ExpectedResult = true)]
    [TestCase(3, ExpectedResult = true)]
    [TestCase(-11, ExpectedResult = true)]
    [TestCase(11, ExpectedResult = true)]
    public bool IsOdd(int value)
    {
        return value.IsOdd();
    }
}
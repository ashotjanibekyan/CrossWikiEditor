using CrossWikiEditor.Utils;

namespace CrossWikiEditor.Tests.Utils;

public class CollectionExtensionsTests
{
    [Test]
    public void ToObservableCollection_ShouldReturnEmptyCollection_WhenEnumerableIsEmpty()
    {
        // arrange
        var nums = new List<object>();

        // act
        var result = nums.ToObservableCollection();

        // assert
        result.Should().BeEquivalentTo(nums);
    }

    [Test]
    public void ToObservableCollection_ShouldReturnFullCollection_WhenEnumerableIsValid()
    {
        // arrange
        var nums = new List<int> { 1, 2, 3, 4, 5 };

        // act
        var result = nums.ToObservableCollection();

        // assert
        result.Should().BeEquivalentTo(nums);
    }
}
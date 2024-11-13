namespace CrossWikiEditor.Tests.Utils;

public sealed class CollectionExtensionsTests
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
        var nums = new List<int> {1, 2, 3, 4, 5};

        // act
        var result = nums.ToObservableCollection();

        // assert
        result.Should().BeEquivalentTo(nums);
    }

    [Test]
    public void RandomSubset_ShouldThrowArgumentNullException_WhenCollectionIsNull()
    {
        // arrange
        List<int>? list = null;

        // act

        // assert
        Assert.Throws<ArgumentNullException>(() => list.RandomSubset(1));
    }

    [Test]
    public void RandomSubset_ShouldReturnOutOfBoundException_WhenSubsetSizeIsLargerThanCollectionSize()
    {
        // arrange
        var list = new List<int> {3, 4, 2};

        // act

        // assert
        ArgumentOutOfRangeException? exception = Assert.Throws<ArgumentOutOfRangeException>(() => list.RandomSubset(4));
        exception.Message.Should().Be("Count should be between 0 and the size of the source list. (Parameter 'count')");
    }

    [Test]
    public void RandomSubset_ShouldReturnSubsetOfGivenSize()
    {
        // arrange
        var list = new List<int> {2, 12, 42, 7, -5};

        // act
        List<int> result = list.RandomSubset(3);

        // assert
        result.Should().BeSubsetOf(list);
    }
}
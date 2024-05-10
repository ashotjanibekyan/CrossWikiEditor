namespace CrossWikiEditor.Tests.ViewModels;

public sealed class AlertViewModelTests : BaseTest
{
    private AlertViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new AlertViewModel("title", "content");
    }

    [Test]
    public void Constructor_Should_SetProps()
    {
        // arrange
        _sut = new AlertViewModel("title", "content");

        // act

        // assert
        _sut.Title.Should().Be("title");
        _sut.ContentText.Should().Be("content");
    }

    [Test]
    public void OkCommand_ShouldCloseWithFalseValue()
    {
        // arrange

        // act
        _sut.OkCommand.Execute(_dialog);

        // assert
        _dialog.Received(1).Close(false);
    }
}
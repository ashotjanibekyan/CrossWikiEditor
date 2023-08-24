using CrossWikiEditor.ViewModels;
using NSubstitute;

namespace CrossWikiEditor.Tests.ViewModels;

public class AlertViewModelTests : BaseTest
{
    private AlertViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new AlertViewModel();
    }

    [Test]
    public void OkCommand_ShouldCloseWithFalseValue()
    {
        // arrange

        // act
        _sut.OkCommand.Execute(_dialog).Subscribe();

        // assert
        _dialog.Received(1).Close(false);
    }
}
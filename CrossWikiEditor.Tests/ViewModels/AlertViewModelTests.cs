using CrossWikiEditor.ViewModels;
using NSubstitute;

namespace CrossWikiEditor.Tests.ViewModels;

public class AlertViewModelTests
{
    private AlertViewModel _sut;
    private IDialog _dialog;

    [SetUp]
    public void SetUp()
    {
        _sut = new AlertViewModel();
        _dialog = Substitute.For<IDialog>();
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
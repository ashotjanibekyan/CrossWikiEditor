namespace CrossWikiEditor.Tests.ViewModels;

public sealed class PromptViewModelTests : BaseTest
{
    private PromptViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new PromptViewModel("title", "text")
        {
            IsNumeric = true
        };
    }

    [Test]
    public void OkCommand_ClosesDialogWithValue([Values(42, 1, 3, 74, 0, -1)] int value)
    {
        // arrange
        _sut.Value = value;

        // act
        _sut.OkCommand.Execute(_dialog);

        // assert
        _dialog.Received(1).Close(value);
    }

    [Test]
    public void CancelCommand_ClosesDialogWithNullValue([Values(42, 1, 3, 74, 0, -1)] int value)
    {
        // arrange
        _sut.Value = value;
        IDialog dialog = Substitute.For<IDialog>();

        // act
        _sut.CancelCommand.Execute(dialog);

        // assert
        dialog.Received(1).Close(null);
    }
}
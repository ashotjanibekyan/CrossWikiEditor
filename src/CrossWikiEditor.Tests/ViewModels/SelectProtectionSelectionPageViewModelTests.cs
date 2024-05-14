namespace CrossWikiEditor.Tests.ViewModels;

public sealed class SelectProtectionSelectionPageViewModelTests : BaseTest
{
    private SelectProtectionSelectionPageViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new SelectProtectionSelectionPageViewModel();
    }

    [TestCase(0, "edit", 3, "")]
    [TestCase(1, "move", 1, "sysop")]
    [TestCase(2, "edit|move", 2, "autoconfirmed|sysop")]
    [TestCase(3, "", 0, "autoconfirmed")]
    public void OkCommand_ShouldReturnTypeAndLevel(int type, string expectedType, int level, string expectedLevel)
    {
        // arrange
        _sut.ProtectionType = type;
        _sut.ProtectionLevel = level;
        IDialog dialog = Substitute.For<IDialog>();

        // act
        _sut.OkCommand.Execute(dialog);

        // assert
        dialog.Received(1).Close(Arg.Is<(string, string)>(tpl => tpl.Item1 == expectedType && tpl.Item2 == expectedLevel));
    }
}
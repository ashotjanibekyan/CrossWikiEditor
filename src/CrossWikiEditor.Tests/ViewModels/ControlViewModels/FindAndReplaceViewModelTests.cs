using CrossWikiEditor.Core.ViewModels.ControlViewModels;

namespace CrossWikiEditor.Tests.ViewModels.ControlViewModels;

public class FindAndReplaceViewModelTests : BaseTest
{
    private FindAndReplaceViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new FindAndReplaceViewModel([]);
    }

    [Test]
    public void Constructor_InitializesProperties()
    {
        _sut.NormalFindAndReplaceRules.Should().NotBeNull();
        _sut.NormalFindAndReplaceRules.Should().HaveCount(1);
        _sut.IgnoreLinks.Should().BeFalse();
        _sut.IgnoreMore.Should().BeFalse();
        _sut.AddToSummary.Should().BeFalse();
    }

    [Test]
    public void ShouldAddNewRule_WhenLastRuleAddedFind()
    {
        // arrange

        // act
        _sut.NormalFindAndReplaceRules.Last().Find = "fwe";

        // assert
        _sut.NormalFindAndReplaceRules.Should().HaveCount(2);
    }

    [Test]
    public void ShouldNotAddNewRule_WhenLastRuleDidNotAddFind()
    {
        // arrange
        NormalFindAndReplaceRule last = _sut.NormalFindAndReplaceRules.Last();

        // act
        last.ReplaceWith = "few";
        last.Comment = "fwe";
        last.Minor = false;
        last.SingleLine = true;
        last.Regex = true;
        last.AfterFixes = false;
        last.Enabled = true;

        // assert
        _sut.NormalFindAndReplaceRules.Should().HaveCount(1);
    }

    [Test]
    public void SaveCommand_ShouldCloseWithRules()
    {
        // arrange
        _sut.NormalFindAndReplaceRules.Last().Find = "find1";
        _sut.NormalFindAndReplaceRules.Last().Find = "find2";

        // act
        _sut.SaveCommand.Execute(_dialog);

        // assert
        _dialog.Close(_sut.NormalFindAndReplaceRules);
    }
}
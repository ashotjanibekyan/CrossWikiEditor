using CrossWikiEditor.Core.ViewModels.ControlViewModels;

namespace CrossWikiEditor.Tests.ViewModels.ControlViewModels;

public class OptionsViewModelTests : BaseTest
{
    private OptionsViewModel _sut;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new OptionsViewModel(_settingsService, _dialogService);
    }

    [Test]
    public void OpenNormalFindAndReplaceDialogCommand_ShouldSetNormalFindAndReplaceRules_WhenDialogReturnsSuccessfully()
    {
        // arrange
        var rules = new NormalFindAndReplaceRules(new[]
        {
            new NormalFindAndReplaceRule("f", "w", false, true, true, true, false, true, false, "fwe")
        }) {AddToSummary = true, IgnoreLinks = true, IgnoreMore = false};
        _dialogService.ShowDialog<NormalFindAndReplaceRules>(Arg.Any<FindAndReplaceViewModel>())
            .Returns(rules);

        // act
        _sut.OpenNormalFindAndReplaceDialogCommand.Execute(null);

        // assert
        _sut.NormalFindAndReplaceRules.Should().BeSameAs(rules);
    }

    [Test]
    public void OpenNormalFindAndReplaceDialogCommand_ShouldDoNothing_WhenDialogReturnsUnsuccessfully()
    {
        // arrange
        var rules = new NormalFindAndReplaceRules(new[]
        {
            new NormalFindAndReplaceRule("f", "w", false, true, true, true, false, true, false, "fwe")
        }) {AddToSummary = true, IgnoreLinks = true, IgnoreMore = false};
        _dialogService.ShowDialog<NormalFindAndReplaceRules>(Arg.Any<FindAndReplaceViewModel>())
            .Returns(rules);
        _sut.OpenNormalFindAndReplaceDialogCommand.Execute(null);
        _dialogService.ShowDialog<NormalFindAndReplaceRules>(Arg.Any<FindAndReplaceViewModel>())
            .Returns(null as NormalFindAndReplaceRules);

        // act
        _sut.OpenNormalFindAndReplaceDialogCommand.Execute(null);

        // assert
        _sut.NormalFindAndReplaceRules.Should().BeSameAs(rules);
    }
}
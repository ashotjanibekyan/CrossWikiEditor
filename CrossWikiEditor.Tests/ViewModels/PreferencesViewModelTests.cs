using CrossWikiEditor.Messages;
using CrossWikiEditor.Settings;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.Tests.ViewModels;

public class PreferencesViewModelTests : BaseTest
{
    private PreferencesViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new PreferencesViewModel(_userPreferencesService, _messenger);
    }

    [Test]
    public void SaveCommand_ShouldSendProjectChangedMessage(
        [Values(ProjectEnum.Mediawiki, ProjectEnum.Commons, ProjectEnum.Wikia)]
        ProjectEnum project)
    {
        // arrange
        _sut.SelectedProject = project;
        
        // act
        _sut.SaveCommand.Execute(_dialog);

        // assert
        _messenger.Received(1).Send(Arg.Is<ProjectChangedMessage>(x => x.Value == project));
        _dialog.Received(1).Close(true);
    }

    [Test]
    public void SaveCommand_ShouldSendLanguageCodeChangedMessage(
        [Values("hy", "hyw", "en")]
        string language)
    {
        // arrange
        _sut.SelectedLanguage = language;
        
        // act
        _sut.SaveCommand.Execute(_dialog);

        // assert
        _messenger.Received(1).Send(Arg.Is<LanguageCodeChangedMessage>(x => x.Value == language));
        _dialog.Received(1).Close(true);
    }

    [Test]
    public void CancelCommand_ShouldCloseWithoutSendingMessage()
    {
        // arrange

        // act
        _sut.CancelCommand.Execute(_dialog);

        // assert
        _dialog.Received(1).Close(false);
        _messenger.Received(0).Send(Arg.Any<LanguageCodeChangedMessage>());
        _messenger.Received(0).Send(Arg.Any<ProjectChangedMessage>());
    }
}
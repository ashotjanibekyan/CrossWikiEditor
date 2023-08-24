using CrossWikiEditor.Messages;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.Tests.ViewModels;

public class StatusBarViewModelTests : BaseTest
{
    private StatusBarViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _messageBus.Listen<NewAccountLoggedInMessage>().Returns(Substitute.For<IObservable<NewAccountLoggedInMessage>>());
        _messageBus.Listen<ProjectChangedMessage>().Returns(Substitute.For<IObservable<ProjectChangedMessage>>());
        _messageBus.Listen<LanguageCodeChangedMessage>().Returns(Substitute.For<IObservable<LanguageCodeChangedMessage>>());
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _userPreferencesService, _messageBus);
    }

    [Test]
    public void CurrentWiki_ShouldComeFromCurrentPref()
    {
        // arrange
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs
        {
            Project = ProjectEnum.Wikipedia,
            LanguageCode = "hyw"
        });

        // act
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _userPreferencesService, _messageBus);

        // assert
        _sut.CurrentWiki.Should().Be($"hyw:Wikipedia");
    }

    [Test]
    public void CurrentWikiClickedCommand_ShouldOpenPreferencesView()
    {
        // arrange
        var preferencesViewModel = new PreferencesViewModel(_userPreferencesService, _messageBus);
        _dialogService.ShowDialog<bool>(Arg.Any<PreferencesViewModel>()).Returns(true);
        _viewModelFactory.GetPreferencesViewModel().Returns(preferencesViewModel);

        // act
        _sut.CurrentWikiClickedCommand.Execute().Subscribe();

        // assert
        Received.InOrder(() =>
        {
            _viewModelFactory.GetPreferencesViewModel();
            _dialogService.Received(1).ShowDialog<bool>(preferencesViewModel);
        });
    }


    [Test]
    public void UsernameClickedCommand_ShouldOpenPreferencesView()
    {
        // arrange
        var profilesViewModel = new ProfilesViewModel(_fileDialogService, _dialogService, _profileRepository, _userService, _userPreferencesService,
            _messageBus);
        _dialogService.ShowDialog<bool>(Arg.Any<PreferencesViewModel>()).Returns(true);
        _viewModelFactory.GetProfilesViewModel().Returns(profilesViewModel);

        // act
        _sut.UsernameClickedCommand.Execute().Subscribe();

        // assert
        Received.InOrder(() =>
        {
            _viewModelFactory.GetProfilesViewModel();
            _dialogService.Received(1).ShowDialog<bool>(profilesViewModel);
        });
    }
}
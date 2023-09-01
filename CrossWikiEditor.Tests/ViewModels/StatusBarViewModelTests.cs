using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Settings;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.Tests.ViewModels;

public class StatusBarViewModelTests : BaseTest
{
    private StatusBarViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _userPreferencesService, _messenger);
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
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _userPreferencesService, _messenger);

        // assert
        _sut.CurrentWiki.Should().Be($"hyw:Wikipedia");
    }

    [Test]
    public void CurrentWikiClickedCommand_ShouldOpenPreferencesView()
    {
        // arrange
        var preferencesViewModel = new PreferencesViewModel(_userPreferencesService, _messenger);
        _dialogService.ShowDialog<bool>(Arg.Any<PreferencesViewModel>()).Returns(true);
        _viewModelFactory.GetPreferencesViewModel().Returns(preferencesViewModel);

        // act
        _sut.CurrentWikiClickedCommand.Execute(null);

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
            _messenger);
        _dialogService.ShowDialog<bool>(Arg.Any<PreferencesViewModel>()).Returns(true);
        _viewModelFactory.GetProfilesViewModel().Returns(profilesViewModel);

        // act
        _sut.UsernameClickedCommand.Execute(null);

        // assert
        Received.InOrder(() =>
        {
            _viewModelFactory.GetProfilesViewModel();
            _dialogService.Received(1).ShowDialog<bool>(profilesViewModel);
        });
    }

    [Test]
    public void Constructor_ShouldSubscribeToNewAccountLoggedInMessage()
    {
        MessageHandler<object, NewAccountLoggedInMessage> handler = null;

        // Act
        var vm = new StatusBarViewModel(_viewModelFactory, _dialogService, _userPreferencesService, _messenger);

        // assert
        _messenger.Received(1).Register(vm, Arg.Any<MessageHandler<object, NewAccountLoggedInMessage>>());
    }

    [Test]
    public void Constructor_ShouldSubscribeToProjectChangedMessage()
    {
        MessageHandler<object, ProjectChangedMessage> handler = null;

        // Act
        var vm = new StatusBarViewModel(_viewModelFactory, _dialogService, _userPreferencesService, _messenger);

        // assert
        _messenger.Received(1).Register(vm, Arg.Any<MessageHandler<object, ProjectChangedMessage>>());
    }

    [Test]
    public void Constructor_ShouldSubscribeToLanguageCodeChangedMessage()
    {
        MessageHandler<object, LanguageCodeChangedMessage> handler = null;

        // Act
        var vm = new StatusBarViewModel(_viewModelFactory, _dialogService, _userPreferencesService, _messenger);

        // assert
        _messenger.Received(1).Register(vm, Arg.Any<MessageHandler<object, LanguageCodeChangedMessage>>());
    }
}
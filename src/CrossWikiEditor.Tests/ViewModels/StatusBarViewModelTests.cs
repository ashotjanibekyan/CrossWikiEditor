using CommunityToolkit.Mvvm.Messaging;

namespace CrossWikiEditor.Tests.ViewModels;

public sealed class StatusBarViewModelTests : BaseTest
{
    private StatusBarViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _settingsService, _messenger);
        _messenger.Received().Register(_sut, Arg.Any<MessageHandler<object, NewAccountLoggedInMessage>>());
        _messenger.Received().Register(_sut, Arg.Any<MessageHandler<object, ProjectChangedMessage>>());
        _messenger.Received().Register(_sut, Arg.Any<MessageHandler<object, LanguageCodeChangedMessage>>());
    }

    [Test]
    public void Messanger_ShouldUpdateUsername_WhenNewAccountLoggedInMessageReceived()
    {
        // arrange
        var messenger = new MessengerWrapper(WeakReferenceMessenger.Default);
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _settingsService, messenger);

        // act
        messenger.Send(new NewAccountLoggedInMessage(new Profile
        {
            Username = "this is a new username"
        }));

        // assert
        _sut.Username.Should().Be("this is a new username");
    }

    [Test]
    public void Messanger_ShouldUpdateLanguageCode_WhenLanguageCodeChangedMessageReceived()
    {
        // arrange
        var messenger = new MessengerWrapper(WeakReferenceMessenger.Default);
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _settingsService, messenger);

        // act
        messenger.Send(new LanguageCodeChangedMessage("de"));

        // assert
        _sut.LanguageCode.Should().Be("de");
    }

    [Test]
    public void Messanger_ShouldUpdateProject_WhenProjectChangedInMessageReceived()
    {
        // arrange
        var messenger = new MessengerWrapper(WeakReferenceMessenger.Default);
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _settingsService, messenger);

        // act
        messenger.Send(new ProjectChangedMessage(ProjectEnum.Incubator));

        // assert
        _sut.Project.Should().Be("Incubator");
    }

    [Test]
    public void CurrentWiki_ShouldComeFromCurrentPref()
    {
        // arrange
        _settingsService.GetCurrentSettings().Returns(new UserSettings
        {
            UserWiki = new UserWiki("hyw", ProjectEnum.Wikipedia)
        });

        // act
        _sut = new StatusBarViewModel(_viewModelFactory, _dialogService, _settingsService, _messenger);

        // assert
        _sut.CurrentWiki.Should().Be("hyw:Wikipedia");
    }

    [Test]
    public void CurrentWikiClickedCommand_ShouldOpenPreferencesView()
    {
        // arrange
        var preferencesViewModel = new PreferencesViewModel(_settingsService, _messenger);
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
        var profilesViewModel = new ProfilesViewModel(_fileDialogService, _dialogService, _profileRepository, _userService, _settingsService,
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
        // Act
        var vm = new StatusBarViewModel(_viewModelFactory, _dialogService, _settingsService, _messenger);

        // assert
        _messenger.Received(1).Register(vm, Arg.Any<MessageHandler<object, NewAccountLoggedInMessage>>());
    }

    [Test]
    public void Constructor_ShouldSubscribeToProjectChangedMessage()
    {
        // Act
        var vm = new StatusBarViewModel(_viewModelFactory, _dialogService, _settingsService, _messenger);

        // assert
        _messenger.Received(1).Register(vm, Arg.Any<MessageHandler<object, ProjectChangedMessage>>());
    }

    [Test]
    public void Constructor_ShouldSubscribeToLanguageCodeChangedMessage()
    {
        // Act
        var vm = new StatusBarViewModel(_viewModelFactory, _dialogService, _settingsService, _messenger);

        // assert
        _messenger.Received(1).Register(vm, Arg.Any<MessageHandler<object, LanguageCodeChangedMessage>>());
    }
}
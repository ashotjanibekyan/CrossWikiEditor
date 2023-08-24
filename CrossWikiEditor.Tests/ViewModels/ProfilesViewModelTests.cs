using Avalonia.Controls.Documents;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.ViewModels;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using WikiClient;

namespace CrossWikiEditor.Tests.ViewModels;

public class ProfilesViewModelTests : BaseTest
{
    private ProfilesViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _profileRepository.GetAll().Returns(new List<Profile>());
        _sut = new ProfilesViewModel(_fileDialogService, _dialogService, _profileRepository, _userService, _userPreferencesServic, _messageBus);
    }

    [Test]
    public void LoginCommand_ShouldReturn_WhenSelectedAccountIsNull()
    {
        // arrange
        _sut.SelectedProfile = null;

        // act
        _sut.LoginCommand.Execute().Subscribe();

        // assert
        _userPreferencesServic.Received(0).GetCurrentPref();
        _messageBus.Received(0).SendMessage(Arg.Any<object>());
        _userService.Received(0).Login(Arg.Any<Profile>(), Arg.Any<Site>());
    }

    [Test]
    public void LoginCommand_ShouldLogin_WhenSelectedUserIsValid()
    {
        // arrange
        var profile = new Profile()
        {
            Username = "username",
            Password = "password",
        };
        _sut.SelectedProfile = profile;
        _userPreferencesServic.GetCurrentPref().Returns(new UserPrefs()
        {
            LanguageCode = "hy",
            Project = ProjectEnum.Wikipedia
        });
        _userService.Login(Arg.Any<Profile>(), Arg.Any<Site>()).Returns(Result.Success());

        // act
        _sut.LoginCommand.Execute().Subscribe();

        // assert
        _userService.Received(1).Login(Arg.Is<Profile>(p => p.Username == profile.Username && p.Password == profile.Password),
            Arg.Is<Site>(s => s.Domain == "https://hy.wikipedia.org/w/api.php?"));
        _messageBus.Received(1).SendMessage(Arg.Any<NewAccountLoggedInMessage>());
        _dialogService.Received(0).Alert(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void LoginCommand_ShouldAlertDefaultMessage_WhenLoginIsUnSuccessfulAndThereIsNoMessage()
    {
        // arrange
        var profile = new Profile()
        {
            Username = "username",
            Password = "password",
        };
        _sut.SelectedProfile = profile;
        _userPreferencesServic.GetCurrentPref().Returns(new UserPrefs()
        {
            LanguageCode = "hy",
            Project = ProjectEnum.Wikipedia
        });
        _userService.Login(Arg.Any<Profile>(), Arg.Any<Site>()).Returns(Result.Failure(string.Empty));

        // act
        _sut.LoginCommand.Execute().Subscribe();

        // assert
        _userService.Received(1).Login(Arg.Is<Profile>(p => p.Username == profile.Username && p.Password == profile.Password),
            Arg.Is<Site>(s => s.Domain == "https://hy.wikipedia.org/w/api.php?"));
        _messageBus.Received(0).SendMessage(Arg.Any<NewAccountLoggedInMessage>());
        _dialogService.Received(1).Alert("Login Attempt Unsuccessful",
            "Login Attempt Unsuccessful: Please ensure an active internet connection and verify the accuracy of your provided username and password.");
    }

    [Test]
    public void LoginCommand_ShouldAlertErrorMessage_WhenLoginIsUnSuccessful()
    {
        // arrange
        var profile = new Profile()
        {
            Username = "username",
            Password = "password",
        };
        _sut.SelectedProfile = profile;
        _userPreferencesServic.GetCurrentPref().Returns(new UserPrefs()
        {
            LanguageCode = "hy",
            Project = ProjectEnum.Wikipedia
        });
        _userService.Login(Arg.Any<Profile>(), Arg.Any<Site>()).Returns(Result.Failure("this is an error message"));

        // act
        _sut.LoginCommand.Execute().Subscribe();

        // assert
        _userService.Received(1).Login(Arg.Is<Profile>(p => p.Username == profile.Username && p.Password == profile.Password),
            Arg.Is<Site>(s => s.Domain == "https://hy.wikipedia.org/w/api.php?"));
        _messageBus.Received(0).SendMessage(Arg.Any<NewAccountLoggedInMessage>());
        _dialogService.Received(1).Alert("Login Attempt Unsuccessful", "this is an error message");
    }

    [Test]
    public void AddCommand_ShouldOpenAddOrEditProfileViewModel()
    {
        // arrange

        // act
        _sut.AddCommand.Execute().Subscribe();

        // assert
        _dialogService.Received(1)
            .ShowDialog<bool>(Arg.Is<AddOrEditProfileViewModel>(vm => !vm.IsEdit));
    }

    [Test]
    public void AddCommand_ShouldUpdateProfiles_WhenAddOrEditProfileViewModelReturnsTrue()
    {
        // arrange
        var newProfiles = new List<Profile>()
        {
            new Profile()
            {
                Username = "username",
                Password = "Qwer1234"
            },
            new Profile()
            {
                Username = "username2",
                Password = "Qwer1234"
            },
        };
        _dialogService.ShowDialog<bool>(Arg.Is<AddOrEditProfileViewModel>(vm => !vm.IsEdit)).Returns(true);
        _profileRepository.ClearReceivedCalls();
        _profileRepository.GetAll().Returns(newProfiles);
        bool didProfileChange = false;
        _sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(_sut.Profiles))
            {
                didProfileChange = true;
            }
        };
        
        // act
        _sut.AddCommand.Execute().Subscribe();

        // assert
        _profileRepository.Received(1).GetAll();
        didProfileChange.Should().BeTrue();
        _sut.Profiles.Should().BeEquivalentTo(newProfiles);

    }
}
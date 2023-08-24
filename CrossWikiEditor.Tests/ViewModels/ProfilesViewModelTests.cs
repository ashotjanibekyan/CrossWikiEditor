using CrossWikiEditor.Messages;
using CrossWikiEditor.Models;
using CrossWikiEditor.Utils;
using CrossWikiEditor.ViewModels;
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
        _sut = new ProfilesViewModel(_fileDialogService, _dialogService, _profileRepository, _userService, _userPreferencesService, _messageBus);
        _profileRepository.ClearReceivedCalls();
    }

    [Test]
    public void LoginCommand_ShouldReturn_WhenSelectedAccountIsNull()
    {
        // arrange
        _sut.SelectedProfile = null;

        // act
        _sut.LoginCommand.Execute().Subscribe();

        // assert
        _userPreferencesService.Received(0).GetCurrentPref();
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
            Password = "password"
        };
        _sut.SelectedProfile = profile;
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs()
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
    public void LoginCommand_ShouldSetCurrentUserPref_WhenProfileHasDefaultPref()
    {
        // arrange
        var profile = new Profile()
        {
            Username = "username",
            Password = "password",
            DefaultSettingsPath = "some/settings/path/file.xml"
        };
        _sut.SelectedProfile = profile;
        var userPrefs = new UserPrefs()
        {
            LanguageCode = "hy",
            Project = ProjectEnum.Wikipedia
        };
        _userPreferencesService.GetUserPref(profile.DefaultSettingsPath).Returns(userPrefs);
        _userService.Login(Arg.Any<Profile>(), Arg.Any<Site>()).Returns(Result.Success());

        // act
        _sut.LoginCommand.Execute().Subscribe();

        // assert
        _userPreferencesService.Received(1)
            .SetCurrentPref(userPrefs);
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
            Password = "password"
        };
        _sut.SelectedProfile = profile;
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs()
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
            Password = "password"
        };
        _sut.SelectedProfile = profile;
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs()
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
            new()
            {
                Username = "username",
                Password = "Qwer1234"
            },
            new()
            {
                Username = "username2",
                Password = "Qwer1234"
            }
        };
        _dialogService.ShowDialog<bool>(Arg.Is<AddOrEditProfileViewModel>(vm => !vm.IsEdit)).Returns(true);
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

    [Test]
    public void EditCommand_ShouldReturn_WhenSelectedProfileIsNull()
    {
        // arrange
        _sut.SelectedProfile = null;

        // act
        _sut.EditCommand.Execute().Subscribe();

        // assert
        _dialogService.DidNotReceive().ShowDialog<bool>(Arg.Any<AddOrEditProfileViewModel>());
        _profileRepository.DidNotReceive().GetAll();
    }

    [Test]
    public void EditCommand_ShouldOpenAddOrEditProfileViewModel_WhenSelectedProfileIsNotNull()
    {
        // arrange
        Profile? profile = Fakers.ProfileFaker.Generate();
        _sut.SelectedProfile = profile;

        // act
        _sut.EditCommand.Execute().Subscribe();

        // assert
        _dialogService.Received(1).ShowDialog<bool>(Arg.Is<AddOrEditProfileViewModel>(vm =>
            vm.Username == profile.Username && vm.DefaultSettingsPath == profile.DefaultSettingsPath && vm.Notes == profile.Notes &&
            vm.Password == profile.Password && vm.ShouldSavePassword == profile.IsPasswordSaved &&
            vm.ShouldSelectDefaultSettings == !string.IsNullOrEmpty(profile.DefaultSettingsPath)));
    }

    [Test]
    public void EditCommand_ShouldUpdateProfiles_WhenAddOrEditProfileDialogReturnsTrue()
    {
        // arrange
        List<Profile> profiles = Fakers.ProfileFaker.Generate(5);
        _sut.SelectedProfile = profiles.First();
        _dialogService.ShowDialog<bool>(Arg.Any<ViewModelBase>()).Returns(true);
        _profileRepository.GetAll().Returns(profiles);

        // act
        _sut.EditCommand.Execute().Subscribe();

        // assert
        _sut.Profiles.Should().BeEquivalentTo(profiles);
    }

    [Test]
    public void EditCommand_ShouldNotUpdateProfiles_WhenAddOrEditProfileDialogReturnsFalse()
    {
        // arrange
        List<Profile> profiles = Fakers.ProfileFaker.Generate(5);
        _sut.SelectedProfile = profiles.First();
        _dialogService.ShowDialog<bool>(Arg.Any<ViewModelBase>()).Returns(false);
        _profileRepository.GetAll().Returns(profiles);

        // act
        _sut.EditCommand.Execute().Subscribe();

        // assert
        _sut.Profiles.Should().NotBeEquivalentTo(profiles);
    }

    [Test]
    public void DeleteCommand_ShouldReturn_WhenSelectedProfileIsNull()
    {
        // arrange
        List<Profile> profiles = Fakers.ProfileFaker.Generate(4);
        _sut.SelectedProfile = null;
        _sut.Profiles = profiles.ToObservableCollection();

        // act
        _sut.DeleteCommand.Execute().Subscribe();

        // assert
        _profileRepository.DidNotReceive().Delete(Arg.Any<int>());
        _profileRepository.DidNotReceive().GetAll();
        _sut.Profiles.Should().BeEquivalentTo(profiles);
    }

    [Test]
    public void DeleteCommand_ShouldDeleteProfile_WhenSelectedProfileIsNotNull()
    {
        // arrange
        List<Profile> profiles = Fakers.ProfileFaker.Generate(4);
        _sut.SelectedProfile = profiles[0];
        _profileRepository.GetAll().Returns(profiles);

        // act
        _sut.DeleteCommand.Execute().Subscribe();

        // assert
        Received.InOrder(() =>
        {
            _profileRepository.Delete(profiles[0].Id);
            _profileRepository.GetAll();
        });
        _sut.Profiles.Should().BeEquivalentTo(profiles);
    }

    [Test]
    public void QuickLoginCommand_ShouldLogin_WhenPasswordAndUsernameArePresent()
    {
        // arrange
        _sut.Username = "username";
        _sut.Password = "Qwer1234";
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs()
        {
            LanguageCode = "hyw",
            Project = ProjectEnum.Wikipedia
        });
        _userService.Login(Arg.Any<Profile>(), Arg.Any<Site>()).Returns(Result.Success());

        // act
        _sut.QuickLoginCommand.Execute().Subscribe();

        // assert
        _userService.Received(1).Login(Arg.Is<Profile>(p => p.Username == "username" && p.Password == "Qwer1234"),
            Arg.Is<Site>(s => s.Domain == "https://hyw.wikipedia.org/w/api.php?"));
    }

    [Test]
    [Combinatorial]
    public void QuickLoginCommand_ShouldReturn_WhenUsernameOrPasswordIsMissing(
        [Values("username", "", " ", null)] string username,
        [Values("password", "", " ", null)] string password)
    {
        // arrange
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        _sut.Username = username;
        _sut.Password = password;

        // act
        _sut.QuickLoginCommand.Execute().Subscribe();

        // assert
        _userService.DidNotReceiveWithAnyArgs().Login(default, default);
    }

    [Test]
    public void QuickLoginCommand_ShouldSendAccountLoggedInMessage_WhenLoggedInSuccessfully()
    {
        // arrange
        _sut.Username = "username";
        _sut.Password = "Qwer1234";
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs()
        {
            LanguageCode = "hyw",
            Project = ProjectEnum.Wikipedia
        });
        _userService.Login(Arg.Any<Profile>(), Arg.Any<Site>()).Returns(Result.Success());

        // act
        _sut.QuickLoginCommand.Execute().Subscribe();

        // assert
        _messageBus.Received(1).SendMessage(Arg.Any<NewAccountLoggedInMessage>());
        _dialogService.DidNotReceive().Alert(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void QuickLoginCommand_ShouldAlertUser_WhenLoggedInUnsuccessfully()
    {
        // arrange
        _sut.Username = "username";
        _sut.Password = "Qwer1234";
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs()
        {
            LanguageCode = "hyw",
            Project = ProjectEnum.Wikipedia
        });
        _userService.Login(Arg.Any<Profile>(), Arg.Any<Site>()).Returns(Result.Failure("Password is wrong"));

        // act
        _sut.QuickLoginCommand.Execute().Subscribe();

        // assert
        _dialogService.Received(1).Alert("Login Attempt Unsuccessful", "Password is wrong");
    }

    [TestCase("")]
    [TestCase("  ")]
    [TestCase(null)]
    public void QuickLoginCommand_ShouldAlertDefaultMessage__WhenLoginIsUnsuccessfulAndThereIsNoMessage(string errorMessage)
    {
        // arrange
        _sut.Username = "username";
        _sut.Password = "Qwer1234";
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs()
        {
            LanguageCode = "hyw",
            Project = ProjectEnum.Wikipedia
        });
        _userService.Login(Arg.Any<Profile>(), Arg.Any<Site>()).Returns(Result.Failure(errorMessage));

        // act
        _sut.QuickLoginCommand.Execute().Subscribe();

        // assert
        _dialogService.Received(1).Alert("Login Attempt Unsuccessful",
            "Login Attempt Unsuccessful: Please ensure an active internet connection and verify the accuracy of your provided username and password.");
    }
}
namespace CrossWikiEditor.Tests.Services;

public sealed class ViewModelFactoryTests : BaseTest
{
    private ViewModelFactory _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new ViewModelFactory(
            _fileDialogService,
            _dialogService,
            _profileRepository,
            _wikiClientCache,
            _userService,
            _settingsService,
            _messenger,
            new TextFileListProvider(
                _fileDialogService,
                _systemService,
                _settingsService,
                _wikiClientCache
            )
        );
    }

    [Test]
    public void GetProfilesViewModel_ReturnsDifferentObjectEachTime()
    {
        // arrange

        // act
        ProfilesViewModel obj1 = _sut.GetProfilesViewModel();
        ProfilesViewModel obj2 = _sut.GetProfilesViewModel();

        // assert
        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();
        obj1.Should().NotBe(obj2);
    }

    [Test]
    public void GetPreferencesViewModel_ReturnsDifferentObjectEachTime()
    {
        // arrange

        // act
        PreferencesViewModel obj1 = _sut.GetPreferencesViewModel();
        PreferencesViewModel obj2 = _sut.GetPreferencesViewModel();

        // assert
        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();
        obj1.Should().NotBe(obj2);
    }
}
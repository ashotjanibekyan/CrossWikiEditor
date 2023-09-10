namespace CrossWikiEditor.Tests.Repositories;

public class SimpleJsonProfileRepositoryTests
{
    private SimpleJsonProfileRepository _sut;

    [SetUp]
    public void SetUp()
    {
        (byte[] key, byte[] iv) = StringEncryptionService.GenerateKeyAndIv("wioejfwoiOWwei;:WEIOF");
        IStringEncryptionService stringEncryptionService = new StringEncryptionService(key, iv);
        _sut = new SimpleJsonProfileRepository(stringEncryptionService);
        File.Delete("profiles.json");
    }

    [Test]
    public void Get_ShouldReturnNull_WhenAccountDoesNotExist()
    {
        // arrange
        Profile? profile = Fakers.ProfileFaker.Generate();
        _sut.Insert(profile);

        // act
        Profile? result = _sut.Get(3);

        // assert
        result.Should().BeNull();
    }

    [Test]
    public void Insert_Get_Should_Return_Same_Profile()
    {
        // arrange
        List<Profile>? profiles = Fakers.ProfileFaker.Generate(3);
        _sut.Insert(profiles[0]);
        _sut.Insert(profiles[1]);
        _sut.Insert(profiles[2]);

        // act
        Profile? result = _sut.Get(1);

        // assert
        result.Should().BeEquivalentTo(profiles[1]);
    }

    [Test]
    public void Delete_Should_Remove_Profile()
    {
        // arrange
        List<Profile>? profiles = Fakers.ProfileFaker.Generate(3);
        _sut.Insert(profiles[0]);
        _sut.Insert(profiles[1]);
        _sut.Insert(profiles[2]);

        // act
        _sut.Delete(1);
        Profile? result = _sut.Get(1);

        // assert
        result.Should().BeNull();
    }

    [Test]
    public void Update_ShouldDoNothing_WhenProfileDoesNotExist()
    {
        // arrange
        List<Profile>? profiles = Fakers.ProfileFaker.Generate(3);
        _sut.Insert(profiles[0]);
        _sut.Insert(profiles[1]);
        _sut.Insert(profiles[2]);

        // act
        _sut.Update(new Profile
        {
            Id = 23,
            DefaultSettingsPath = "few",
            Username = "fweew",
            IsPasswordSaved = true,
            Notes = "hth3",
            Password = "0r23v"
        });

        // assert
        List<Profile> existingProfiles = _sut.GetAll();
        existingProfiles.Should().BeEquivalentTo(profiles);
    }
    
    
    [Test]
    public void Update_ShouldUpdate_WhenProfileExists()
    {
        // arrange
        List<Profile>? profiles = Fakers.ProfileFaker.Generate(3);
        _sut.Insert(profiles[0]);
        _sut.Insert(profiles[1]);
        _sut.Insert(profiles[2]);

        // act
        var newProfile = new Profile
        {
            Id = 0,
            DefaultSettingsPath = "few",
            Username = "fweew",
            IsPasswordSaved = true,
            Notes = "hth3",
            Password = "0r23v"
        };
        _sut.Update(newProfile);

        // assert
        Profile? updatedProfile = _sut.Get(0);
        updatedProfile.Should().BeEquivalentTo(newProfile);
    }

    [Test]
    public void GetAll_ShouldReturnEmptyList_WhenThereAreNoAccounts()
    {
        // arrange

        // act
        List<Profile> existingProfiles = _sut.GetAll();
        
        // assert
        existingProfiles.Count.Should().Be(0);
    }

    [Test]
    public void GetAll_ShouldReturnAllProfiles()
    {
        // arrange
        List<Profile>? profiles = Fakers.ProfileFaker.Generate(3);
        _sut.Insert(profiles[0]);
        _sut.Insert(profiles[1]);
        _sut.Insert(profiles[2]);

        // act
        List<Profile> existingProfiles = _sut.GetAll();
        
        // assert
        existingProfiles.Count.Should().Be(3);
        existingProfiles[0].Should().BeEquivalentTo(profiles[0]);
        existingProfiles[1].Should().BeEquivalentTo(profiles[1]);
        existingProfiles[2].Should().BeEquivalentTo(profiles[2]);
    }
}
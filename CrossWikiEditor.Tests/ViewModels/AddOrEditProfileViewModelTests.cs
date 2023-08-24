using Avalonia.Platform.Storage;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.ViewModels;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CrossWikiEditor.Tests;

public class AddOrEditProfileViewModelTests
{
    private AddOrEditProfileViewModel _sut;
    private IFileDialogService _fileDialogServiceMock;
    private IProfileRepository _profileRepository;
    private IDialog _dialog;

    [SetUp]
    public void SetUp()
    {
        _fileDialogServiceMock = Substitute.For<IFileDialogService>();
        _profileRepository = Substitute.For<IProfileRepository>();
        _dialog = Substitute.For<IDialog>();
        _sut = new AddOrEditProfileViewModel(_fileDialogServiceMock, _profileRepository, -1);
    }

    [Test]
    public void BrowseCommand_ShouldSetDefaultSettingsPath_WhenValidFileIsSelected()
    {
        // arrange
        _fileDialogServiceMock
            .OpenFilePickerAsync("Select settings file", false,
                Arg.Is<List<FilePickerFileType>>(
                    x => x.Count == 1 && x[0].Patterns != null && x[0].Patterns.Count == 1 && x[0].Patterns[0] == "*.xml"))
            .Returns(new[] {"some/valid/file.xml"});

        // act
        _sut.BrowseCommand.Execute().Subscribe();

        // assert
        _sut.DefaultSettingsPath.Should().Be("some/valid/file.xml");
    }

    [Test]
    public void BrowseCommand_ShouldNotSetDefaultSettingsPath_WhenNoFileIsSelected()
    {
        // arrange
        _fileDialogServiceMock
            .OpenFilePickerAsync("Select settings file", false,
                Arg.Is<List<FilePickerFileType>>(
                    x => x.Count == 1 && x[0].Patterns != null && x[0].Patterns.Count == 1 && x[0].Patterns[0] == "*.xml"))
            .Returns(Array.Empty<string>());
        var initialDefaultSettingsPath = _sut.DefaultSettingsPath;
        
        // act
        _sut.BrowseCommand.Execute().Subscribe();

        // assert
        _sut.DefaultSettingsPath.Should().Be(initialDefaultSettingsPath);
    }

    [Test]
    public void BrowseCommand_ShouldNotSetDefaultSettingsPath_WhenBrowseDialogIsCanceled()
    {
        // arrange
        _fileDialogServiceMock
            .OpenFilePickerAsync("Select settings file", false,
                Arg.Is<List<FilePickerFileType>>(
                    x => x.Count == 1 && x[0].Patterns != null && x[0].Patterns.Count == 1 && x[0].Patterns[0] == "*.xml"))
            .ReturnsNull();
        var initialDefaultSettingsPath = _sut.DefaultSettingsPath;
        
        // act
        _sut.BrowseCommand.Execute().Subscribe();

        // assert
        _sut.DefaultSettingsPath.Should().Be(initialDefaultSettingsPath);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void SaveCommand_ShouldCloseWithFalseResult_WhenUsernameIsNullOrEmpty(string? username)
    {
        // arrange
        _sut.Username = username;
        _sut.DefaultSettingsPath = "some/path/file.xml";
        _sut.Password = "Qwer1234";

        // act
        _sut.SaveCommand.Execute(_dialog).Subscribe();

        // assert
        _dialog.Received(1).Close(false);
    }
    
    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void SaveCommand_ShouldCloseWithFalseResult_WhenPasswordIsNullOrEmpty_And_ShouldSavePassword(string? password)
    {
        // arrange
        _sut.Password = password;
        _sut.ShouldSavePassword = true;
        _sut.Username = "username";
        _sut.DefaultSettingsPath = "some/path/file.xml";

        // act
        _sut.SaveCommand.Execute(_dialog).Subscribe();

        // assert
        _dialog.Received(1).Close(false);
    }
        
    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void SaveCommand_ShouldCloseWithFalseResult_WhenDefaultSettingsPathIsNullOrEmpty_And_ShouldSelectDefaultSettings(string? defaultSettingsPath)
    {
        // arrange
        _sut.DefaultSettingsPath = defaultSettingsPath;
        _sut.ShouldSelectDefaultSettings = true;
        _sut.Password = "Qwer1234";
        _sut.Username = "username";

        // act
        _sut.SaveCommand.Execute(_dialog).Subscribe();

        // assert
        _dialog.Received(1).Close(false);
    }

    [TestCase("username",  "Qwer1234", true, "some/path/file.xml", true, "this is a note")]
    [TestCase("username",  "Qwer1234", false, "some/path/file.xml", true, "this is a note")]
    [TestCase("username",  "", false, "some/path/file.xml", false, "this is a note")]
    [TestCase("username",  "Qwer1234", false, "", false, "")]
    public void SaveCommand_ShouldInsertProfile_WhenIdIsMinus1(string username,
        string password,
        bool shouldSavePassword,
        string defaultSettingsPath,
        bool shouldSelectDefaultSettings,
        string notes)
    {
        // arrange
        _sut = new AddOrEditProfileViewModel(_fileDialogServiceMock, _profileRepository, -1)
        {
            Username = username,
            Password = shouldSavePassword ? password : string.Empty,
            ShouldSavePassword = shouldSavePassword,
            DefaultSettingsPath = defaultSettingsPath,
            ShouldSelectDefaultSettings = shouldSelectDefaultSettings,
            Notes = notes
        };

        // act
        _sut.SaveCommand.Execute(_dialog).Subscribe();

        // assert
        _profileRepository.Received(1).Insert(Arg.Is<Profile>(p =>
            p.Username == username && p.DefaultSettingsPath == (shouldSelectDefaultSettings ? defaultSettingsPath : "") && p.IsPasswordSaved == shouldSavePassword &&
            p.Password == (shouldSavePassword ? password : "") && p.Notes == notes));
        _profileRepository.DidNotReceive().Update(Arg.Any<Profile>());
        _dialog.Received(1).Close(true);
    }
    
    
    [TestCase(0, "username",  "Qwer1234", true, "some/path/file.xml", true, "this is a note")]
    [TestCase(1, "username",  "Qwer1234", false, "some/path/file.xml", true, "this is a note")]
    [TestCase(2, "username",  "", false, "some/path/file.xml", false, "this is a note")]
    [TestCase(3, "username",  "Qwer1234", false, "", false, "")]
    public void SaveCommand_ShouldUpdateProfile_WhenIdIsNonNegatives(int id,
        string username,
        string password,
        bool shouldSavePassword,
        string defaultSettingsPath,
        bool shouldSelectDefaultSettings,
        string notes)
    {
        // arrange
        _sut = new AddOrEditProfileViewModel(_fileDialogServiceMock, _profileRepository, id)
        {
            Username = username,
            Password = shouldSavePassword ? password : string.Empty,
            ShouldSavePassword = shouldSavePassword,
            DefaultSettingsPath = defaultSettingsPath,
            ShouldSelectDefaultSettings = shouldSelectDefaultSettings,
            Notes = notes
        };

        // act
        _sut.SaveCommand.Execute(_dialog).Subscribe();

        // assert
        _profileRepository.Received(1).Update(Arg.Is<Profile>(p =>
            p.Username == username && p.DefaultSettingsPath == (shouldSelectDefaultSettings ? defaultSettingsPath : "") && p.IsPasswordSaved == shouldSavePassword &&
            p.Password == (shouldSavePassword ? password : "") && p.Notes == notes && p.Id == id));
        _dialog.Received(1).Close(true);
    }

    [Test]
    public void CancelCommand_ShouldCloseWithFalseResult()
    {
        // arrange

        // act
        _sut.CancelCommand.Execute(_dialog).Subscribe();

        // assert
        _dialog.Received(1).Close(false);
    }
}
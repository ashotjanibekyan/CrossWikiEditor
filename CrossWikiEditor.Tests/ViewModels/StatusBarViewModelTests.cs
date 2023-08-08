using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.ViewModels;
using Moq;

namespace CrossWikiEditor.Tests.ViewModels;

public class StatusBarViewModelTests
{
    private readonly Mock<IFileDialogService> _fileDialogService = new();
    private readonly Mock<IDialogService> _dialogServiceMock = new();
    private readonly Mock<IProfileRepository> _profileRepositoryMock = new();
    private readonly Mock<ICredentialService> _credentialServiceMock = new();
    private StatusBarViewModel? _statusBarViewModel;
    
    
    [SetUp]
    public void SetUp()
    {
        _statusBarViewModel = new StatusBarViewModel(_fileDialogService.Object, _dialogServiceMock.Object, _profileRepositoryMock.Object, _credentialServiceMock.Object);
    }

    [Test]
    public void UsernameClickedCommand_OpensProfilesWindow()
    {
        // arrange
        _profileRepositoryMock
            .Setup(profileService => profileService.GetAll())
            .Returns(new List<Profile>());
        
        // act
        _statusBarViewModel?.UsernameClickedCommand.Execute().Subscribe();

        // assert
        _dialogServiceMock
            .Verify(dialogService => dialogService.ShowDialog<bool>(It.IsAny<ProfilesViewModel>()));
    }
}
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.ViewModels;
using NSubstitute;

namespace CrossWikiEditor.Tests.ViewModels;

public class StatusBarViewModelTests
{
    private IFileDialogService _fileDialogService = Substitute.For<IFileDialogService>();
    private IDialogService _dialogServiceMock = Substitute.For<IDialogService>();
    private IProfileRepository _profileRepositoryMock = Substitute.For<IProfileRepository>();
    private IUserService _userServiceMock = Substitute.For<IUserService>();
    private StatusBarViewModel _statusBarViewModel;

    [SetUp]
    public void SetUp()
    {
        _statusBarViewModel = new StatusBarViewModel(_fileDialogService, _dialogServiceMock, _profileRepositoryMock, _userServiceMock);
    }

    [Test]
    public void UsernameClickedCommand_OpensProfilesWindow()
    {
        // arrange
        _profileRepositoryMock.GetAll().Returns(new List<Profile>());

        // act
        _statusBarViewModel.UsernameClickedCommand.Execute().Subscribe();

        // assert
        _dialogServiceMock.Received().ShowDialog<bool>(Arg.Any<ProfilesViewModel>());
    }
}

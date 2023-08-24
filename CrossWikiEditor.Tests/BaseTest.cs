using System.Collections.Immutable;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using NSubstitute;

namespace CrossWikiEditor.Tests;

public abstract class BaseTest
{
    protected IFileDialogService _fileDialogService;
    protected IDialogService _dialogService;
    protected IProfileRepository _profileRepository;
    protected IUserService _userService;
    protected IUserPreferencesService _userPreferencesServic;
    protected IDialog _dialog;

    public void SetUpServices()
    {
        _fileDialogService = Substitute.For<IFileDialogService>();
        _dialogService = Substitute.For<IDialogService>();
        _profileRepository = Substitute.For<IProfileRepository>();
        _userService = Substitute.For<IUserService>();
        _userPreferencesServic = Substitute.For<IUserPreferencesService>();
        _dialog = Substitute.For<IDialog>();
    }
}
using System.Collections.Immutable;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using NSubstitute;
using ReactiveUI;

namespace CrossWikiEditor.Tests;

public abstract class BaseTest
{
    protected IFileDialogService _fileDialogService;
    protected IDialogService _dialogService;
    protected IProfileRepository _profileRepository;
    protected IUserService _userService;
    protected IUserPreferencesService _userPreferencesService;
    protected IViewModelFactory _viewModelFactory;
    protected IDialog _dialog;
    protected IMessageBus _messageBus;

    public void SetUpServices()
    {
        _fileDialogService = Substitute.For<IFileDialogService>();
        _dialogService = Substitute.For<IDialogService>();
        _profileRepository = Substitute.For<IProfileRepository>();
        _userService = Substitute.For<IUserService>();
        _userPreferencesService = Substitute.For<IUserPreferencesService>();
        _viewModelFactory = Substitute.For<IViewModelFactory>();
        _dialog = Substitute.For<IDialog>();
        _messageBus = Substitute.For<IMessageBus>();
    }
}
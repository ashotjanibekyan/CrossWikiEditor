using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using ReactiveUI;

namespace CrossWikiEditor.Tests;

public abstract class BaseTest
{
    protected IFileDialogService _fileDialogService;
    protected IDialogService _dialogService;
    protected IProfileRepository _profileRepository;
    protected IUserPreferencesService _userPreferencesService;
    protected IViewModelFactory _viewModelFactory;
    protected IDialog _dialog;
    protected IMessageBus _messageBus;
    
    protected IUserService _userService;
    protected IPageService _pageService;
    

    protected void SetUpServices()
    {
        _fileDialogService = Substitute.For<IFileDialogService>();
        _dialogService = Substitute.For<IDialogService>();
        _profileRepository = Substitute.For<IProfileRepository>();
        _userService = Substitute.For<IUserService>();
        _pageService = Substitute.For<IPageService>();
        _userPreferencesService = Substitute.For<IUserPreferencesService>();
        _viewModelFactory = Substitute.For<IViewModelFactory>();
        _dialog = Substitute.For<IDialog>();
        _messageBus = Substitute.For<IMessageBus>();
    }
}
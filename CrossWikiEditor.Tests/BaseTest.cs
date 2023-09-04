using Avalonia.Input.Platform;
using CrossWikiEditor.Core;
using CrossWikiEditor.Core.Repositories;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using Serilog;

namespace CrossWikiEditor.Tests;

public abstract class BaseTest
{
    protected IFileDialogService _fileDialogService;
    protected ISystemService _systemService;
    protected IDialogService _dialogService;
    protected IProfileRepository _profileRepository;
    protected IUserPreferencesService _userPreferencesService;
    protected IViewModelFactory _viewModelFactory;
    protected IDialog _dialog;
    protected IMessengerWrapper _messenger;
    protected IClipboard _clipboard;
    protected ILogger _logger;

    protected IUserService _userService;
    protected IPageService _pageService;
    protected IWikiClientCache _wikiClientCache;


    protected void SetUpServices()
    {
        _fileDialogService = Substitute.For<IFileDialogService>();
        _systemService = Substitute.For<ISystemService>();
        _dialogService = Substitute.For<IDialogService>();
        _profileRepository = Substitute.For<IProfileRepository>();
        _userPreferencesService = Substitute.For<IUserPreferencesService>();
        _viewModelFactory = Substitute.For<IViewModelFactory>();
        _dialog = Substitute.For<IDialog>();
        _messenger = Substitute.For<IMessengerWrapper>();
        _clipboard = Substitute.For<IClipboard>();
        _logger = Substitute.For<ILogger>();

        _userService = Substitute.For<IUserService>();
        _pageService = Substitute.For<IPageService>();
        _wikiClientCache = Substitute.For<IWikiClientCache>();
    }
}
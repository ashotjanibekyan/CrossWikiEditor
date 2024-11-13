using Avalonia.Input.Platform;
using CrossWikiEditor.Core.Services.WikiServices;
using Serilog;

namespace CrossWikiEditor.Tests;

[Parallelizable]
public abstract class BaseTest
{
    protected ICategoryService _categoryService;
    protected IClipboard _clipboard;
    protected IDialog _dialog;
    protected IDialogService _dialogService;
    protected IFileDialogService _fileDialogService;
    protected ILogger _logger;
    protected IMessengerWrapper _messenger;
    protected IPageService _pageService;
    protected IProfileRepository _profileRepository;
    protected ISettingsService _settingsService;
    protected ISystemService _systemService;

    protected IUserService _userService;
    protected IViewModelFactory _viewModelFactory;
    protected IWikiClientCache _wikiClientCache;

    protected void SetUpServices()
    {
        _fileDialogService = Substitute.For<IFileDialogService>();
        _systemService = Substitute.For<ISystemService>();
        _dialogService = Substitute.For<IDialogService>();
        _profileRepository = Substitute.For<IProfileRepository>();
        _settingsService = Substitute.For<ISettingsService>();
        _settingsService.GetCurrentSettings().Returns(new UserSettings());
        _viewModelFactory = Substitute.For<IViewModelFactory>();
        _dialog = Substitute.For<IDialog>();
        _messenger = Substitute.For<IMessengerWrapper>();
        _clipboard = Substitute.For<IClipboard>();
        _logger = Substitute.For<ILogger>();

        _userService = Substitute.For<IUserService>();
        _pageService = Substitute.For<IPageService>();
        _categoryService = Substitute.For<ICategoryService>();
        _wikiClientCache = Substitute.For<IWikiClientCache>();
    }
}
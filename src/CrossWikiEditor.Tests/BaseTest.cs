using Avalonia.Input.Platform;
using CrossWikiEditor.Core.Services.WikiServices;
using Serilog;

namespace CrossWikiEditor.Tests;

[Parallelizable]
public abstract class BaseTest
{
    protected IFileDialogService _fileDialogService;
    protected ISystemService _systemService;
    protected IDialogService _dialogService;
    protected IProfileRepository _profileRepository;
    protected ISettingsService _settingsService;
    protected IViewModelFactory _viewModelFactory;
    protected IDialog _dialog;
    protected IMessengerWrapper _messenger;
    protected IClipboard _clipboard;
    protected ILogger _logger;

    protected IUserService _userService;
    protected IPageService _pageService;
    protected ICategoryService _categoryService;
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
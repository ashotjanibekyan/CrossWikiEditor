using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Repositories;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.Utils.Extensions;
using CrossWikiEditor.Core.ViewModels;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.Services;

public interface IViewModelFactory
{
    ProfilesViewModel GetProfilesViewModel();
    PreferencesViewModel GetPreferencesViewModel();
    Task<FilterViewModel> GetFilterViewModel();
    Task<SelectNamespacesViewModel> GetSelectNamespacesViewModel(bool isMultiselect = true);
    Task<SelectNamespacesAndRedirectFilterViewModel> GetSelectNamespacesAndRedirectFilterViewModel(bool isIncludeRedirectsVisible = true);
    SelectProtectionSelectionPageViewModel GetSelectProtectionSelectionPageViewModel();
    DatabaseScannerViewModel GetDatabaseScannerViewModel();
}

public sealed class ViewModelFactory : IViewModelFactory
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly IWikiClientCache _wikiClientCache;
    private readonly IUserService _userService;
    private readonly ISettingsService _settingsService;
    private readonly IMessengerWrapper _messenger;
    private readonly TextFileListProvider _textFileListProvider;

    public ViewModelFactory(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IWikiClientCache wikiClientCache,
        IUserService userService,
        ISettingsService settingsService,
        IMessengerWrapper messenger,
        TextFileListProvider textFileListProvider)
    {
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _profileRepository = profileRepository;
        _wikiClientCache = wikiClientCache;
        _userService = userService;
        _settingsService = settingsService;
        _messenger = messenger;
        _textFileListProvider = textFileListProvider;
    }

    public ProfilesViewModel GetProfilesViewModel()
    {
        return new ProfilesViewModel(_fileDialogService, _dialogService, _profileRepository, _userService, _settingsService, _messenger);
    }

    public PreferencesViewModel GetPreferencesViewModel()
    {
        return new PreferencesViewModel(_settingsService, _messenger);
    }

    public async Task<FilterViewModel> GetFilterViewModel()
    {
        WikiSite? site = await _wikiClientCache.GetWikiSite(_settingsService.CurrentApiUrl);
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();

        return new FilterViewModel(
            namespaces.Where(x => x.Id.IsEven()).ToList(),
            namespaces.Where(x => x.Id.IsOdd()).ToList(),
            _textFileListProvider);
    }

    public async Task<SelectNamespacesViewModel> GetSelectNamespacesViewModel(bool isMultiselect = true)
    {
        WikiSite site = await _wikiClientCache.GetWikiSite(_settingsService.CurrentApiUrl);
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new SelectNamespacesViewModel([.. namespaces], isMultiselect);
    }

    public async Task<SelectNamespacesAndRedirectFilterViewModel> GetSelectNamespacesAndRedirectFilterViewModel(bool isIncludeRedirectsVisible = true)
    {
        WikiSite site = await _wikiClientCache.GetWikiSite(_settingsService.CurrentApiUrl);
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new SelectNamespacesAndRedirectFilterViewModel([.. namespaces])
        {
            IsIncludeRedirectsVisible = isIncludeRedirectsVisible
        };
    }

    public SelectProtectionSelectionPageViewModel GetSelectProtectionSelectionPageViewModel()
    {
        return new SelectProtectionSelectionPageViewModel();
    }

    public DatabaseScannerViewModel GetDatabaseScannerViewModel()
    {
        return new DatabaseScannerViewModel(_settingsService, _wikiClientCache, _fileDialogService);
    }
}
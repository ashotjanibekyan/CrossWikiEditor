﻿namespace CrossWikiEditor.Core.Services;

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

public sealed class ViewModelFactory(
    IFileDialogService fileDialogService,
    IDialogService dialogService,
    IProfileRepository profileRepository,
    IWikiClientCache wikiClientCache,
    IUserService userService,
    ISettingsService settingsService,
    IMessengerWrapper messenger,
    TextFileListProvider textFileListProvider)
    : IViewModelFactory
{
    public ProfilesViewModel GetProfilesViewModel()
    {
        return new ProfilesViewModel(fileDialogService, dialogService, profileRepository, userService, settingsService, messenger);
    }

    public PreferencesViewModel GetPreferencesViewModel()
    {
        return new PreferencesViewModel(settingsService, messenger);
    }

    public async Task<FilterViewModel> GetFilterViewModel()
    {
        WikiSite? site = await wikiClientCache.GetWikiSite(settingsService.CurrentApiUrl);
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();

        return new FilterViewModel(
            namespaces.Where(x => x.Id.IsEven()).ToList(),
            namespaces.Where(x => x.Id.IsOdd()).ToList(),
            textFileListProvider);
    }

    public async Task<SelectNamespacesViewModel> GetSelectNamespacesViewModel(bool isMultiselect = true)
    {
        WikiSite site = await wikiClientCache.GetWikiSite(settingsService.CurrentApiUrl);
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new SelectNamespacesViewModel([.. namespaces], isMultiselect);
    }

    public async Task<SelectNamespacesAndRedirectFilterViewModel> GetSelectNamespacesAndRedirectFilterViewModel(bool isIncludeRedirectsVisible = true)
    {
        WikiSite site = await wikiClientCache.GetWikiSite(settingsService.CurrentApiUrl);
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
        return new DatabaseScannerViewModel(settingsService, wikiClientCache, fileDialogService);
    }
}
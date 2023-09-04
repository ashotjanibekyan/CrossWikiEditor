using CrossWikiEditor.Core.ListProviders;
using CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;
using CrossWikiEditor.Core.Repositories;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.ViewModels;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.Services;

public interface IViewModelFactory
{
    ProfilesViewModel GetProfilesViewModel();
    PreferencesViewModel GetPreferencesViewModel();
    Task<FilterViewModel> GetFilterViewModel();
    Task<SelectNamespacesViewModel> GetSelectNamespacesViewModel();
    Task<WhatLinksHereOptionsViewModel> GetWhatLinksHereOptionsViewModel();
    Task<SpecialPageListProviderSelectorViewModel> GetSpecialPageListProviderSelectorViewModel();
}

public class ViewModelFactory(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IWikiClientCache wikiClientCache,
        IUserService userService,
        IUserPreferencesService userPreferencesService,
        IMessengerWrapper messenger,
        IEnumerable<ISpecialPageListProvider> specialPageListProviders,
        TextFileListProvider textFileListProvider)
    : IViewModelFactory
{
    public ProfilesViewModel GetProfilesViewModel()
    {
        return new ProfilesViewModel(fileDialogService, dialogService, profileRepository, userService, userPreferencesService, messenger);
    }

    public PreferencesViewModel GetPreferencesViewModel()
    {
        return new PreferencesViewModel(userPreferencesService, messenger);
    }

    public async Task<FilterViewModel> GetFilterViewModel()
    {
        WikiSite? site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();

        return new FilterViewModel(
            namespaces.Where(x => x.Id.IsEven()).ToList(),
            namespaces.Where(x => x.Id.IsOdd()).ToList(),
            textFileListProvider);
    }

    public async Task<SelectNamespacesViewModel> GetSelectNamespacesViewModel()
    {
        WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new SelectNamespacesViewModel(namespaces.ToList());
    }

    public async Task<WhatLinksHereOptionsViewModel> GetWhatLinksHereOptionsViewModel()
    {
        WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new WhatLinksHereOptionsViewModel(namespaces.ToList());
    }

    public async Task<SpecialPageListProviderSelectorViewModel> GetSpecialPageListProviderSelectorViewModel()
    {
        WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
        WikiNamespace[] namespaces = site.Namespaces.Where(x => x.Id >= 0).Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new SpecialPageListProviderSelectorViewModel(specialPageListProviders.ToList(), namespaces.ToList());
    }
}
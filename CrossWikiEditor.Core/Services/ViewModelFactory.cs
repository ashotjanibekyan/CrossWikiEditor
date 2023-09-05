using CrossWikiEditor.Core.ListProviders;
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
    Task<SelectNamespacesViewModel> GetSelectNamespacesViewModel(bool isMultiselect = true);
    Task<SelectNamespacesAndRedirectFilterViewModel> GetSelectNamespacesAndRedirectFilterViewModel(bool isIncludeRedirectsVisible = true);
}

public sealed class ViewModelFactory(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IWikiClientCache wikiClientCache,
        IUserService userService,
        IUserPreferencesService userPreferencesService,
        IMessengerWrapper messenger,
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

    public async Task<SelectNamespacesViewModel> GetSelectNamespacesViewModel(bool isMultiselect = true)
    {
        WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new SelectNamespacesViewModel(namespaces.ToList(), isMultiselect);
    }

    public async Task<SelectNamespacesAndRedirectFilterViewModel> GetSelectNamespacesAndRedirectFilterViewModel(bool isIncludeRedirectsVisible = true)
    {
        WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new SelectNamespacesAndRedirectFilterViewModel(namespaces.ToList())
        {
            IsIncludeRedirectsVisible = isIncludeRedirectsVisible
        };
    }
}
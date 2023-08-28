using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using CrossWikiEditor.ViewModels;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services;

public interface IViewModelFactory
{
    ProfilesViewModel GetProfilesViewModel();
    PreferencesViewModel GetPreferencesViewModel();
    Task<FilterViewModel> GetFilterViewModel();
    Task<SelectNamespacesViewModel> GetSelectNamespacesViewModel();
}

public class ViewModelFactory(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IWikiClientCache wikiClientCache,
        IUserService userService,
        IUserPreferencesService userPreferencesService,
        IMessenger messenger,
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
        WikiSite? site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        return new SelectNamespacesViewModel(namespaces.ToList());
    }
}
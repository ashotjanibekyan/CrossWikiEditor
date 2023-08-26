﻿using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using CrossWikiEditor.ViewModels;
using ReactiveUI;

namespace CrossWikiEditor.Services;

public interface IViewModelFactory
{
    ProfilesViewModel GetProfilesViewModel();
    PreferencesViewModel GetPreferencesViewModel();
    Task<FilterViewModel> GetFilterViewModel();
}

public class ViewModelFactory : IViewModelFactory
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly IWikiClientCache _wikiClientCache;
    private readonly IUserService _userService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IMessageBus _messageBus;

    public ViewModelFactory(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IWikiClientCache wikiClientCache,
        IUserService userService,
        IUserPreferencesService userPreferencesService,
        IMessageBus messageBus)
    {
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _profileRepository = profileRepository;
        _wikiClientCache = wikiClientCache;
        _userService = userService;
        _userPreferencesService = userPreferencesService;
        _messageBus = messageBus;
    }

    public ProfilesViewModel GetProfilesViewModel()
    {
        return new ProfilesViewModel(_fileDialogService, _dialogService, _profileRepository, _userService, _userPreferencesService, _messageBus);
    }

    public PreferencesViewModel GetPreferencesViewModel()
    {
        return new PreferencesViewModel(_userPreferencesService, _messageBus);
    }

    public async Task<FilterViewModel> GetFilterViewModel()
    {
        var site = await _wikiClientCache.GetWikiSite(_userPreferencesService.GetCurrentPref().UrlApi());
        WikiNamespace[] namespaces = site.Namespaces.Select(x => new WikiNamespace(x.Id, x.CustomName)).ToArray();
        
        return new FilterViewModel(
            subjectNamespaces: namespaces.Where(x => x.Id.IsEven()).ToList(), 
            talkNamespaces: namespaces.Where(x => x.Id.IsOdd()).ToList());
    }
}
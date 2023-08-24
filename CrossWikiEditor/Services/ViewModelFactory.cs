using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.ViewModels;
using ReactiveUI;

namespace CrossWikiEditor.Services;

public interface IViewModelFactory
{
    ProfilesViewModel GetProfilesViewModel();
    PreferencesViewModel GetPreferencesViewModel();
}

public class ViewModelFactory : IViewModelFactory
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly IUserService _userService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IMessageBus _messageBus;

    public ViewModelFactory(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IUserService userService,
        IUserPreferencesService userPreferencesService,
        IMessageBus messageBus)
    {
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _profileRepository = profileRepository;
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
}
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WikiClient;

namespace CrossWikiEditor.ViewModels;

public sealed class ProfilesViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly IUserService _userService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IMessageBus _messageBus;

    public ProfilesViewModel(IFileDialogService fileDialogService,
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
        LoginCommand = ReactiveCommand.CreateFromTask(Login);
        AddCommand = ReactiveCommand.CreateFromTask(Add);
        EditCommand = ReactiveCommand.CreateFromTask(Edit);
        DeleteCommand = ReactiveCommand.Create(Delete);
        QuickLoginCommand = ReactiveCommand.Create(QuickLogin);
        Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
    }


    [Reactive]
    public Profile? SelectedProfile { get; set; }

    [Reactive]
    public ObservableCollection<Profile> Profiles { get; set; }
    
    public ReactiveCommand<Unit, Unit> LoginCommand { get; set; }
    public ReactiveCommand<Unit, Unit> AddCommand { get; set; }
    public ReactiveCommand<Unit, Unit> EditCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }
    public ReactiveCommand<Unit, Unit> QuickLoginCommand { get; set; }
    
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    private async Task Login()
    {
        if (SelectedProfile == null)
        {
            return;
        }
        UserPrefs currentUserPref = _userPreferencesService.GetCurrentPref();
        var site = new Site(currentUserPref.ApiRoot());
        
        Result loginResult = await _userService.Login(SelectedProfile, site);
        if (loginResult is {IsSuccessful: true})
        {
            _messageBus.SendMessage(new NewAccountLoggedInMessage(SelectedProfile));
        }
        else
        {
            if (string.IsNullOrWhiteSpace(loginResult.Error))
            {
                await _dialogService.Alert("Login Attempt Unsuccessful",
                    "Login Attempt Unsuccessful: Please ensure an active internet connection and verify the accuracy of your provided username and password.");
            }
            else
            {
                
                await _dialogService.Alert("Login Attempt Unsuccessful", loginResult.Error);
            }

        }
    }

    private async Task Add()
    {
        if (await _dialogService.ShowDialog<bool>(new AddOrEditProfileViewModel(_fileDialogService, _profileRepository, -1)))
        {
            Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
        }
    }

    private async Task Edit()
    {
        if (SelectedProfile is null)
        {
            return;
        }

        var vm = new AddOrEditProfileViewModel(_fileDialogService, _profileRepository, SelectedProfile.Id)
        {
            Username = SelectedProfile.Username,
            DefaultSettingsPath = SelectedProfile.DefaultSettingsPath,
            Notes = SelectedProfile.Notes,
            Password = SelectedProfile.Password ?? string.Empty,
            ShouldSavePassword = SelectedProfile.IsPasswordSaved,
            ShouldSelectDefaultSettings = !string.IsNullOrEmpty(SelectedProfile.DefaultSettingsPath),
        };
        if (await _dialogService.ShowDialog<bool>(vm))
        {
            Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
        }
    }

    private void Delete()
    {
        if (SelectedProfile is null)
        {
            return;
        }
        _profileRepository.Delete(SelectedProfile.Id);
        SelectedProfile = null;
        Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
    }

    private void QuickLogin()
    {
        throw new System.NotImplementedException();
    }
}
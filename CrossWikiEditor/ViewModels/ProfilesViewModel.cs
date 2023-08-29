using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Settings;

namespace CrossWikiEditor.ViewModels;

public sealed partial class ProfilesViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly IUserService _userService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IMessenger _messenger;

    public ProfilesViewModel(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IUserService userService,
        IUserPreferencesService userPreferencesService,
        IMessenger messenger)
    {
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _profileRepository = profileRepository;
        _userService = userService;
        _userPreferencesService = userPreferencesService;
        _messenger = messenger;
        Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll() ?? new List<Profile>());
    }


    [ObservableProperty] private Profile? _selectedProfile;
    [ObservableProperty] private ObservableCollection<Profile> _profiles;

    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

    [RelayCommand]
    private async Task Login(IDialog dialog)
    {
        if (SelectedProfile == null)
        {
            return;
        }

        await Login(SelectedProfile, dialog);
    }

    [RelayCommand]
    private async Task Add()
    {
        if (await _dialogService.ShowDialog<bool>(new AddOrEditProfileViewModel(_fileDialogService, _profileRepository, -1)))
        {
            Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
        }
    }

    [RelayCommand]
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
            ShouldSelectDefaultSettings = !string.IsNullOrEmpty(SelectedProfile.DefaultSettingsPath)
        };
        if (await _dialogService.ShowDialog<bool>(vm))
        {
            Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
        }
    }

    [RelayCommand]
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

    [RelayCommand]
    private async Task QuickLogin(IDialog dialog)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            return;
        }

        var profile = new Profile()
        {
            Username = Username,
            Password = Password
        };
        await Login(profile, dialog);
    }

    private async Task Login(Profile profile, IDialog dialog)
    {
        UserPrefs currentUserPref = string.IsNullOrEmpty(profile.DefaultSettingsPath)
            ? _userPreferencesService.GetCurrentPref()
            : _userPreferencesService.GetUserPref(profile.DefaultSettingsPath);

        Result loginResult = await _userService.Login(profile, currentUserPref.UrlApi());
        if (loginResult is { IsSuccessful: true })
        {
            _messenger.Send(new NewAccountLoggedInMessage(profile));
            if (!string.IsNullOrEmpty(profile.DefaultSettingsPath))
            {
                _userPreferencesService.SetCurrentPref(currentUserPref);
            }

            dialog.Close(true);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(loginResult?.Error))
            {
                await _dialogService.Alert("Login Attempt Unsuccessful", loginResult.Error);
            }
            else
            {
                await _dialogService.Alert("Login Attempt Unsuccessful",
                    "Login Attempt Unsuccessful: Please ensure an active internet connection and verify the accuracy of your provided username and password.");
            }
        }
    }
}
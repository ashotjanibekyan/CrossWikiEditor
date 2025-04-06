using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Repositories;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class ProfilesViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly IUserService _userService;
    private readonly ISettingsService _settingsService;
    private readonly IMessengerWrapper _messenger;

    public ProfilesViewModel(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IUserService userService,
        ISettingsService settingsService,
        IMessengerWrapper messenger)
    {
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _profileRepository = profileRepository;
        _userService = userService;
        _settingsService = settingsService;
        _messenger = messenger;
        Profiles = new ObservableCollection<Profile>(profileRepository.GetAll() ?? []);
    }

    [ObservableProperty] public partial Profile? SelectedProfile { get; set; }

    [ObservableProperty] public partial ObservableCollection<Profile> Profiles { get; set; }

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

        var profile = new Profile
        {
            Username = Username,
            Password = Password
        };
        await Login(profile, dialog);
    }

    private async Task Login(Profile profile, IDialog dialog)
    {
        UserSettings? currentUserSettings = _settingsService.GetSettingsByPath(profile.DefaultSettingsPath);
        currentUserSettings ??= _settingsService.GetCurrentSettings();

        Result<Unit> loginResult = await _userService.Login(profile, currentUserSettings.GetApiUrl());
        if (loginResult is {IsSuccessful: true})
        {
            _messenger.Send(new NewAccountLoggedInMessage(profile));
            if (!string.IsNullOrEmpty(profile.DefaultSettingsPath))
            {
                _settingsService.SetCurrentSettings(currentUserSettings);
            }

            dialog.Close(true);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(loginResult.ErrorMessage))
            {
                await _dialogService.Alert("Login Attempt Unsuccessful", loginResult.ErrorMessage);
            }
            else
            {
                await _dialogService.Alert("Login Attempt Unsuccessful",
                    "Login Attempt Unsuccessful: Please ensure an active internet connection and verify the accuracy of your provided username and password.");
            }
        }
    }
}
using System.Collections.ObjectModel;
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

public sealed partial class ProfilesViewModel(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IUserService userService,
        IUserPreferencesService userPreferencesService,
        IMessengerWrapper messenger)
    : ViewModelBase
{
    [ObservableProperty] private Profile? _selectedProfile;
    [ObservableProperty] private ObservableCollection<Profile> _profiles = new(profileRepository.GetAll() ?? new List<Profile>());

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
        if (await dialogService.ShowDialog<bool>(new AddOrEditProfileViewModel(fileDialogService, profileRepository, -1)))
        {
            Profiles = new ObservableCollection<Profile>(profileRepository.GetAll());
        }
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (SelectedProfile is null)
        {
            return;
        }

        var vm = new AddOrEditProfileViewModel(fileDialogService, profileRepository, SelectedProfile.Id)
        {
            Username = SelectedProfile.Username,
            DefaultSettingsPath = SelectedProfile.DefaultSettingsPath,
            Notes = SelectedProfile.Notes,
            Password = SelectedProfile.Password ?? string.Empty,
            ShouldSavePassword = SelectedProfile.IsPasswordSaved,
            ShouldSelectDefaultSettings = !string.IsNullOrEmpty(SelectedProfile.DefaultSettingsPath)
        };
        if (await dialogService.ShowDialog<bool>(vm))
        {
            Profiles = new ObservableCollection<Profile>(profileRepository.GetAll());
        }
    }

    [RelayCommand]
    private void Delete()
    {
        if (SelectedProfile is null)
        {
            return;
        }

        profileRepository.Delete(SelectedProfile.Id);
        SelectedProfile = null;
        Profiles = new ObservableCollection<Profile>(profileRepository.GetAll());
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
            ? userPreferencesService.GetCurrentPref()
            : userPreferencesService.GetUserPref(profile.DefaultSettingsPath);

        Result loginResult = await userService.Login(profile, currentUserPref.UrlApi());
        if (loginResult is { IsSuccessful: true })
        {
            messenger.Send(new NewAccountLoggedInMessage(profile));
            if (!string.IsNullOrEmpty(profile.DefaultSettingsPath))
            {
                userPreferencesService.SetCurrentPref(currentUserPref);
            }

            dialog.Close(true);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(loginResult?.Error))
            {
                await dialogService.Alert("Login Attempt Unsuccessful", loginResult.Error);
            }
            else
            {
                await dialogService.Alert("Login Attempt Unsuccessful",
                    "Login Attempt Unsuccessful: Please ensure an active internet connection and verify the accuracy of your provided username and password.");
            }
        }
    }
}
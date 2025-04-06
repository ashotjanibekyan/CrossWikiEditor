using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels.MenuViewModels;

public sealed partial class FileMenuViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IFileDialogService _fileDialogService;
    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;
    private readonly IMessengerWrapper _messenger;

    public FileMenuViewModel(IViewModelFactory viewModelFactory,
        IFileDialogService fileDialogService,
        ISettingsService settingsService,
        IDialogService dialogService,
        IMessengerWrapper messenger)
    {
        _viewModelFactory = viewModelFactory;
        _fileDialogService = fileDialogService;
        _settingsService = settingsService;
        _dialogService = dialogService;
        _messenger = messenger;
    }

    [RelayCommand]
    private void ResetToDefaultSettings()
    {
        _settingsService.SetCurrentSettings(_settingsService.GetDefaultSettings());
    }

    [RelayCommand]
    private async Task OpenSettings()
    {
        string[]? result = await _fileDialogService.OpenFilePickerAsync("Select settings", false, ["*.json"]);
        if (result is {Length: 1})
        {
            string newSettingsPath = result[0];
            try
            {
                UserSettings? newUserSettings = _settingsService.GetSettingsByPath(newSettingsPath) ??
                                                throw new InvalidOperationException("Failed to load the settings");
                _settingsService.SetCurrentSettings(newUserSettings);
            }
            catch (InvalidOperationException)
            {
                await _dialogService.Alert("Failed to load the settings", "Failed to load the settings. Are you sure it is in the correct format?");
            }
        }
    }

    [RelayCommand]
    private void SaveSettings()
    {
        _settingsService.SaveCurrentSettings();
    }

    [RelayCommand]
    private void SaveSettingsAs()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void SaveSettingsAsDefault()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task LoginProfiles()
    {
        await _dialogService.ShowDialog<bool>(_viewModelFactory.GetProfilesViewModel());
    }

    [RelayCommand]
    private void Logout()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void RefreshStatusAndTypos()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void Exit()
    {
        _messenger.Send(new ExitApplicationMessage());
    }
}
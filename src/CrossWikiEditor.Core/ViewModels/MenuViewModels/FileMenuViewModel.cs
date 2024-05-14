namespace CrossWikiEditor.Core.ViewModels.MenuViewModels;

public sealed partial class FileMenuViewModel(
    IViewModelFactory viewModelFactory,
    IFileDialogService fileDialogService,
    ISettingsService settingsService,
    IDialogService dialogService)
{
    [RelayCommand]
    private void ResetToDefaultSettings()
    {
        settingsService.SetCurrentSettings(settingsService.GetDefaultSettings());
    }

    [RelayCommand]
    private async Task OpenSettings()
    {
        string[]? result = await fileDialogService.OpenFilePickerAsync("Select settings", false, ["*.json"]);
        if (result is { Length: 1 })
        {
            string newSettingsPath = result[0];
            try
            {
                UserSettings? newUserSettings = settingsService.GetSettingsByPath(newSettingsPath) ?? throw new InvalidOperationException("Failed to load the settings");
                settingsService.SetCurrentSettings(newUserSettings);
            }
            catch (InvalidOperationException)
            {
                await dialogService.Alert("Failed to load the settings", "Failed to load the settings. Are you sure it is in the correct format?");
            }
        }
    }

    [RelayCommand]
    private void SaveSettings()
    {
        settingsService.SaveCurrentSettings();
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
        await dialogService.ShowDialog<bool>(viewModelFactory.GetProfilesViewModel());
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
}
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Services;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels.MenuViewModels;

public sealed partial class FileMenuViewModel(
    Window mainWindow, 
    IViewModelFactory viewModelFactory, 
    IFileDialogService fileDialogService,
    IUserPreferencesService userPreferencesService, 
    IDialogService dialogService)
{
    [RelayCommand]
    private void ResetToDefaultSettings()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task OpenSettings()
    {
        string[]? result = await fileDialogService.OpenFilePickerAsync("Select settings", false, new List<FilePickerFileType>
        {
            new(null)
            {
                Patterns = new List<string> {"*.xml"}.AsReadOnly()
            }
        });
        if (result is {Length: 1})
        {
            string newSettingsPath = result[0];
            try
            {
                UserPrefs newUserPref = userPreferencesService.GetUserPref(newSettingsPath);
                userPreferencesService.SetCurrentPref(newUserPref);
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
        throw new NotImplementedException();
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

    [RelayCommand]
    private void Exit()
    {
        mainWindow.Close();
    }
}
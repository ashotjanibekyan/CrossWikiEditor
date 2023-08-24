using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CrossWikiEditor.Services;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels.MenuViewModels;

public sealed class FileMenuViewModel
{
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileDialogService;
    private readonly Window _mainWindow;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IViewModelFactory _viewModelFactory;

    public FileMenuViewModel(Window mainWindow, IViewModelFactory viewModelFactory, IFileDialogService fileDialogService,
        IUserPreferencesService userPreferencesService, IDialogService dialogService)
    {
        _mainWindow = mainWindow;
        _viewModelFactory = viewModelFactory;
        _fileDialogService = fileDialogService;
        _userPreferencesService = userPreferencesService;
        _dialogService = dialogService;

        ResetToDefaultSettingsCommand = ReactiveCommand.Create(ResetToDefaultSettings);
        OpenSettingsCommand = ReactiveCommand.CreateFromTask(OpenSettings);
        SaveSettingsCommand = ReactiveCommand.Create(SaveSettings);
        SaveSettingsAsCommand = ReactiveCommand.Create(SaveSettingsAs);
        SaveSettingsAsDefaultCommand = ReactiveCommand.Create(SaveSettingsAsDefault);
        LoginProfilesCommand = ReactiveCommand.CreateFromTask(LoginProfiles);
        LogoutCommand = ReactiveCommand.Create(Logout);
        RefreshStatusAndTyposCommand = ReactiveCommand.Create(RefreshStatusAndTypos);
        ExitCommand = ReactiveCommand.Create(Exit);
    }

    public ReactiveCommand<Unit, Unit> ResetToDefaultSettingsCommand { get; init; }
    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; init; }
    public ReactiveCommand<Unit, Unit> SaveSettingsCommand { get; init; }
    public ReactiveCommand<Unit, Unit> SaveSettingsAsCommand { get; init; }
    public ReactiveCommand<Unit, Unit> SaveSettingsAsDefaultCommand { get; init; }
    public ReactiveCommand<Unit, Unit> LoginProfilesCommand { get; init; }
    public ReactiveCommand<Unit, Unit> LogoutCommand { get; init; }
    public ReactiveCommand<Unit, Unit> RefreshStatusAndTyposCommand { get; init; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; init; }

    private void ResetToDefaultSettings()
    {
        throw new NotImplementedException();
    }

    private async Task OpenSettings()
    {
        string[]? result = await _fileDialogService.OpenFilePickerAsync("Select settings", false, new List<FilePickerFileType>
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
                UserPrefs newUserPref = _userPreferencesService.GetUserPref(newSettingsPath);
                _userPreferencesService.SetCurrentPref(newUserPref);
            }
            catch (InvalidOperationException)
            {
                await _dialogService.Alert("Failed to load the settings", "Failed to load the settings. Are you sure it is in the correct format?");
            }
        }
    }

    private void SaveSettings()
    {
        throw new NotImplementedException();
    }

    private void SaveSettingsAs()
    {
        throw new NotImplementedException();
    }

    private void SaveSettingsAsDefault()
    {
        throw new NotImplementedException();
    }

    private async Task LoginProfiles()
    {
        await _dialogService.ShowDialog<bool>(_viewModelFactory.GetProfilesViewModel());
    }

    private void Logout()
    {
        throw new NotImplementedException();
    }

    private void RefreshStatusAndTypos()
    {
        throw new NotImplementedException();
    }

    private void Exit()
    {
        _mainWindow.Close();
    }
}
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CrossWikiEditor.Services;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class MenuViewModel : ViewModelBase
{
    private readonly Window _mainWindow;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IUserPreferencesService _userPreferencesService;

    public MenuViewModel(Window mainWindow,
        IViewModelFactory viewModelFactory,
        IDialogService dialogService,
        IFileDialogService fileDialogService,
        IUserPreferencesService userPreferencesService)
    {
        _mainWindow = mainWindow;
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        _fileDialogService = fileDialogService;
        _userPreferencesService = userPreferencesService;
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

    private void ResetToDefaultSettings()
    {
        throw new System.NotImplementedException();
    }

    private async Task OpenSettings()
    {
        string[]? result = await _fileDialogService.OpenFilePickerAsync("Select settings", false, new List<FilePickerFileType>
        {
            new(null)
            {
                Patterns = new List<string>{"*.xml"}.AsReadOnly()
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
        throw new System.NotImplementedException();
    }

    private void SaveSettingsAs()
    {
        throw new System.NotImplementedException();
    }

    private void SaveSettingsAsDefault()
    {
        throw new System.NotImplementedException();
    }

    private async Task LoginProfiles()
    {
        await _dialogService.ShowDialog<bool>(_viewModelFactory.GetProfilesViewModel());
    }

    private void Logout()
    {
        throw new System.NotImplementedException();
    }

    private void RefreshStatusAndTypos()
    {
        throw new System.NotImplementedException();
    }

    private void Exit()
    {
        _mainWindow.Close();
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
}

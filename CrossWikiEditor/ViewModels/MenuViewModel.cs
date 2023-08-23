using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class MenuViewModel : ViewModelBase
{
    private readonly Window _mainWindow;

    public MenuViewModel(Window mainWindow)
    {
        _mainWindow = mainWindow;
        ResetToDefaultSettingsCommand = ReactiveCommand.Create(ResetToDefaultSettings);
        OpenSettingsCommand = ReactiveCommand.Create(OpenSettings);
        SaveSettingsCommand = ReactiveCommand.Create(SaveSettings);
        SaveSettingsAsCommand = ReactiveCommand.Create(SaveSettingsAs);
        SaveSettingsAsDefaultCommand = ReactiveCommand.Create(SaveSettingsAsDefault);
        LoginProfilesCommand = ReactiveCommand.Create(LoginProfiles);
        LogoutCommand = ReactiveCommand.Create(Logout);
        RefreshStatusAndTyposCommand = ReactiveCommand.Create(RefreshStatusAndTypos);
        ExitCommand = ReactiveCommand.Create(Exit);
    }

    private void ResetToDefaultSettings()
    {
        throw new System.NotImplementedException();
    }

    private void OpenSettings()
    {
        throw new System.NotImplementedException();
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

    private void LoginProfiles()
    {
        throw new System.NotImplementedException();
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

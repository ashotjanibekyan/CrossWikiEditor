using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using CrossWikiEditor.Services;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class MenuViewModel : ViewModelBase
{
    private readonly Window _mainWindow;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IDialogService _dialogService;

    public MenuViewModel(Window mainWindow, IViewModelFactory viewModelFactory, IDialogService dialogService)
    {
        _mainWindow = mainWindow;
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        ResetToDefaultSettingsCommand = ReactiveCommand.Create(ResetToDefaultSettings);
        OpenSettingsCommand = ReactiveCommand.Create(OpenSettings);
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

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed class StatusBarViewModel : ViewModelBase
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IDialogService _dialogService;
    private readonly IUserPreferencesService _userPreferencesService;
    private ObservableAsPropertyHelper<string> _languageCode;
    private ObservableAsPropertyHelper<string> _project;

    public StatusBarViewModel(IViewModelFactory viewModelFactory, IDialogService dialogService, IUserPreferencesService userPreferencesService)
    {
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        _userPreferencesService = userPreferencesService;
        _languageCode = this.WhenAnyValue(x => x.LanguageCode)
            .Select(x => x)
            .ToProperty(this, x => x.CurrentWiki);
        _project = this.WhenAnyValue(x => x.Project)
            .Select(x => x)
            .ToProperty(this, x => x.CurrentWiki);
        MessageBus.Current.Listen<NewAccountLoggedInMessage>()
            .Subscribe((message) =>
            {
                Username = message.Profile.Username;
            });
        MessageBus.Current.Listen<ProjectChangedMessage>()
            .Subscribe(message =>
            {
                Project = message.Project.ToString();
            });
        MessageBus.Current.Listen<LanguageCodeChangedMessage>()
            .Subscribe(message =>
            {
                LanguageCode = message.LanguageCode;
            });
        UsernameClickedCommand = ReactiveCommand.CreateFromTask(UsernameClicked);
        CurrentWikiClickedCommand = ReactiveCommand.CreateFromTask(CurrentWikiClicked);
        UserPrefs currentPref = userPreferencesService.GetCurrentPref();
        Project = currentPref.Project.ToString();
        LanguageCode = currentPref.LanguageCode;
    }
    
    [Reactive]
    public string Username { get; set; } = "User: ";
    public string CurrentWiki => $"{_languageCode.Value}:{_project.Value}";
    [Reactive]
    private string LanguageCode { get; set; }
    [Reactive]
    private string Project { get; set; }
    public ReactiveCommand<Unit, Unit> UsernameClickedCommand { get; }
    public ReactiveCommand<Unit, Unit> CurrentWikiClickedCommand { get; }

    private async Task UsernameClicked()
    {
        await _dialogService.ShowDialog<bool>(_viewModelFactory.GetProfilesViewModel());
    }
    
    private async Task CurrentWikiClicked()
    {
        await _dialogService.ShowDialog<bool>(_viewModelFactory.GetPreferencesViewModel());
    }
}
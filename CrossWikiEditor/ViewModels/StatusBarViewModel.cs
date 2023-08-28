using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Services;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public sealed partial class StatusBarViewModel : ViewModelBase
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IDialogService _dialogService;
    private readonly IUserPreferencesService _userPreferencesService;

    public StatusBarViewModel(IViewModelFactory viewModelFactory,
        IDialogService dialogService,
        IUserPreferencesService userPreferencesService,
        IMessageBus messageBus)
    {
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        _userPreferencesService = userPreferencesService;
        messageBus.Listen<NewAccountLoggedInMessage>()
            .Subscribe((message) => { Username = message.Profile.Username; });
        messageBus.Listen<ProjectChangedMessage>()
            .Subscribe(message => { Project = message.Project.ToString(); });
        messageBus.Listen<LanguageCodeChangedMessage>()
            .Subscribe(message => { LanguageCode = message.LanguageCode; });
        UserPrefs currentPref = userPreferencesService.GetCurrentPref();
        Project = currentPref.Project.ToString();
        LanguageCode = currentPref.LanguageCode;
    }
    
    public string CurrentWiki => $"{LanguageCode}:{Project}";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentWiki))]
    private string _username = "User: ";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentWiki))]
    private string _languageCode;

    [ObservableProperty] private string _project;

    [RelayCommand]
    private async Task UsernameClicked()
    {
        await _dialogService.ShowDialog<bool>(_viewModelFactory.GetProfilesViewModel());
    }

    [RelayCommand]
    private async Task CurrentWikiClicked()
    {
        await _dialogService.ShowDialog<bool>(_viewModelFactory.GetPreferencesViewModel());
    }
}
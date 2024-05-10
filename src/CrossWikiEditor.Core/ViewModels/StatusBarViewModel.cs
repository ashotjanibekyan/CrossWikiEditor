namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class StatusBarViewModel : ViewModelBase
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IDialogService _dialogService;

    public StatusBarViewModel(IViewModelFactory viewModelFactory,
        IDialogService dialogService,
        IUserPreferencesService userPreferencesService,
        IMessengerWrapper messenger)
    {
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        messenger.Register<NewAccountLoggedInMessage>(this, (_, m) => Username = m.Value.Username);
        messenger.Register<ProjectChangedMessage>(this, (_, m) => Project = m.Value.ToString());
        messenger.Register<LanguageCodeChangedMessage>(this, (_, m) => LanguageCode = m.Value);
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
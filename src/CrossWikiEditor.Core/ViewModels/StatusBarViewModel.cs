namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class StatusBarViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;

    public StatusBarViewModel(IViewModelFactory viewModelFactory,
        IDialogService dialogService,
        ISettingsService settingsService,
        IMessengerWrapper messenger)
    {
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        messenger.Register<NewAccountLoggedInMessage>(this, (_, m) => Username = m.Value.Username);
        messenger.Register<ProjectChangedMessage>(this, (_, m) => Project = m.Value.ToString());
        messenger.Register<LanguageCodeChangedMessage>(this, (_, m) => LanguageCode = m.Value);
        UserSettings currentPref = settingsService.GetCurrentSettings();
        Project = currentPref.UserWiki.Project.ToString();
        LanguageCode = currentPref.UserWiki.LanguageCode ?? "";
    }

    public string CurrentWiki => $"{LanguageCode}:{Project}";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentWiki))]
    public partial string Username { get; set; } = "User: ";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentWiki))]
    public partial string LanguageCode { get; set; }

    [ObservableProperty] public partial string Project { get; set; }

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
namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class PreferencesViewModel : ViewModelBase
{
    private readonly IMessengerWrapper _messenger;
    private readonly ISettingsService _settingsService;

    [ObservableProperty] private ObservableCollection<string> _languages = new(
    [
        "en",
        "hy",
        "hyw",
        "es",
        "ru"
    ]);

    public PreferencesViewModel(ISettingsService settingsService, IMessengerWrapper messenger)
    {
        _settingsService = settingsService;
        _messenger = messenger;
        Alerts = new Alerts
        {
            AmbiguousCitationDates = true,
            EditorsSignatureOrLink = true,
            DeadLinks = true,
            MultipleDefaultSort = true,
            SeeAlsoOutOfPlace = true
        };

        SelectedLanguage = settingsService.GetCurrentSettings().UserWiki.LanguageCode ?? "";
        SelectedProject = settingsService.GetCurrentSettings().UserWiki.Project;
    }

    [ObservableProperty] public partial bool MinimizeToSystray { get; set; }

    [ObservableProperty] public partial bool WarnOnExit { get; set; }

    [ObservableProperty] public partial bool SavePageListWithSettings { get; set; }

    [ObservableProperty] public partial bool LowThreadPriority { get; set; }

    [ObservableProperty] public partial bool PreviewDiffInBotMode { get; set; }

    [ObservableProperty] public partial bool EnableLogging { get; set; }

    [ObservableProperty] public partial ProjectEnum SelectedProject { get; set; } = ProjectEnum.Wikipedia;

    [ObservableProperty] public partial ObservableCollection<ProjectEnum> Projects { get; set; } = new(Enum.GetValues<ProjectEnum>());

    [ObservableProperty] public partial string SelectedLanguage { get; set; } = "en";

    [ObservableProperty] public partial bool SuppressUsingAwb { get; set; }

    [ObservableProperty] public partial bool IgnoreNoBots { get; set; }

    [ObservableProperty] public partial bool EmptyPageListOnProjectChange { get; set; }

    [ObservableProperty] public partial Alerts Alerts { get; set; }

    [RelayCommand]
    private void Save(IDialog dialog)
    {
        _messenger.Send(new ProjectChangedMessage(SelectedProject));
        _messenger.Send(new LanguageCodeChangedMessage(SelectedLanguage));
        dialog.Close(true);
    }

    [RelayCommand]
    private void Cancel(IDialog dialog)
    {
        dialog.Close(false);
    }
}

public partial class Alerts : ObservableObject
{
    [ObservableProperty] public partial bool AmbiguousCitationDates { get; set; }

    [ObservableProperty] public partial bool ContainsSicTag { get; set; }

    [ObservableProperty] public partial bool DabPageWithRef { get; set; }

    [ObservableProperty] public partial bool DeadLinks { get; set; }

    [ObservableProperty] public partial bool DuplicateParametersInWpBannerShell { get; set; }

    [ObservableProperty] public partial bool HasRefAfterReferences { get; set; }

    [ObservableProperty] public partial bool HasFootnotesTemplate { get; set; }

    [ObservableProperty] public partial bool HeadersWithWikilinks { get; set; }

    [ObservableProperty] public partial bool InvalidCitationParameters { get; set; }

    [ObservableProperty] public partial bool LinksWithDoublePipes { get; set; }

    [ObservableProperty] public partial bool LinksWithNoTarget { get; set; }

    [ObservableProperty] public partial bool LongArticleWithStubTag { get; set; }

    [ObservableProperty] public partial bool MultipleDefaultSort { get; set; }

    [ObservableProperty] public partial bool NoCategory { get; set; }

    [ObservableProperty] public partial bool SeeAlsoOutOfPlace { get; set; }

    [ObservableProperty] public partial bool StartsWithHeading { get; set; }

    [ObservableProperty] public partial bool UnbalancedBrackets { get; set; }

    [ObservableProperty] public partial bool UnclosedTags { get; set; }

    [ObservableProperty] public partial bool UnformattedReferences { get; set; }

    [ObservableProperty] public partial bool UnknownParametersInMultipleIssues { get; set; }

    [ObservableProperty] public partial bool UnknownParametersInWpBannerShell { get; set; }

    [ObservableProperty] public partial bool EditorsSignatureOrLink { get; set; }
}
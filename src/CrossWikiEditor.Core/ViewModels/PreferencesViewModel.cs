﻿namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class PreferencesViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IMessengerWrapper _messenger;

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

    [ObservableProperty] private bool _minimizeToSystray;
    [ObservableProperty] private bool _warnOnExit;
    [ObservableProperty] private bool _savePageListWithSettings;
    [ObservableProperty] private bool _lowThreadPriority;
    [ObservableProperty] private bool _previewDiffInBotMode;
    [ObservableProperty] private bool _enableLogging;
    [ObservableProperty] private ProjectEnum _selectedProject = ProjectEnum.Wikipedia;
    [ObservableProperty] private ObservableCollection<ProjectEnum> _projects = new(Enum.GetValues<ProjectEnum>());
    [ObservableProperty] private string _selectedLanguage = "en";

    [ObservableProperty]
    private ObservableCollection<string> _languages = new(
    [
        "en",
        "hy",
        "hyw",
        "es",
        "ru"
    ]);

    [ObservableProperty] private bool _suppressUsingAwb;
    [ObservableProperty] private bool _ignoreNoBots;
    [ObservableProperty] private bool _emptyPageListOnProjectChange;
    [ObservableProperty] private Alerts _alerts;

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
    [ObservableProperty] private bool _ambiguousCitationDates;
    [ObservableProperty] private bool _containsSicTag;
    [ObservableProperty] private bool _dabPageWithRef;
    [ObservableProperty] private bool _deadLinks;
    [ObservableProperty] private bool _duplicateParametersInWpBannerShell;
    [ObservableProperty] private bool _hasRefAfterReferences;
    [ObservableProperty] private bool _hasFootnotesTemplate;
    [ObservableProperty] private bool _headersWithWikilinks;
    [ObservableProperty] private bool _invalidCitationParameters;
    [ObservableProperty] private bool _linksWithDoublePipes;
    [ObservableProperty] private bool _linksWithNoTarget;
    [ObservableProperty] private bool _longArticleWithStubTag;
    [ObservableProperty] private bool _multipleDefaultSort;
    [ObservableProperty] private bool _noCategory;
    [ObservableProperty] private bool _seeAlsoOutOfPlace;
    [ObservableProperty] private bool _startsWithHeading;
    [ObservableProperty] private bool _unbalancedBrackets;
    [ObservableProperty] private bool _unclosedTags;
    [ObservableProperty] private bool _unformattedReferences;
    [ObservableProperty] private bool _unknownParametersInMultipleIssues;
    [ObservableProperty] private bool _unknownParametersInWpBannerShell;
    [ObservableProperty] private bool _editorsSignatureOrLink;
}
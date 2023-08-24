﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Reactive;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed class PreferencesViewModel : ViewModelBase
{
    private readonly IUserPreferencesService _userPreferencesService;

    public PreferencesViewModel(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
        Alerts = new()
        {
            AmbiguousCitationDates = true,
            EditorsSignatureOrLink = true,
            DeadLinks = true,
            MultipleDefaultSort = true,
            SeeAlsoOutOfPlace = true
        };

        SelectedLanguage = userPreferencesService.GetCurrentPref().LanguageCode;
        SelectedProject = userPreferencesService.GetCurrentPref().Project;

        SaveCommand = ReactiveCommand.Create<IDialog>(Save);
        CancelCommand = ReactiveCommand.Create((IDialog dialog) => dialog.Close(false));
    }
    
    public ReactiveCommand<IDialog, Unit> SaveCommand { get; }
    public ReactiveCommand<IDialog, Unit> CancelCommand { get; }
    
    [Reactive]
    public bool MinimizeToSystray { get; set; }

    [Reactive]
    public bool WarnOnExit { get; set; }

    [Reactive]
    public bool SavePageListWithSettings { get; set; }

    [Reactive]
    public bool LowThreadPriority { get; set; }

    [Reactive]
    public bool PreviewDiffInBotMode { get; set; }

    [Reactive]
    public bool EnableLogging { get; set; }

    [Reactive] public ProjectEnum SelectedProject { get; set; } = ProjectEnum.Wikipedia;

    [Reactive]
    public ObservableCollection<ProjectEnum> Projects { get; set; } = new(Enum.GetValues<ProjectEnum>());

    [Reactive] 
    public string SelectedLanguage { get; set; } = "en";
    
    [Reactive]
    public ObservableCollection<string> Languages { get; set; } = new(new List<string>
    {
        "en",
        "hy",
        "hyw",
        "es",
        "ru"
    });
    
    [Reactive]
    public bool SuppressUsingAwb { get; set; }
    
    [Reactive]
    public bool IgnoreNoBots { get; set; }
    
    [Reactive]
    public bool EmptyPageListOnProjectChange { get; set; }

    [Reactive]
    public Alerts Alerts { get; set; }
    
    private void Save(IDialog dialog)
    {
        MessageBus.Current.SendMessage(new ProjectChangedMessage(SelectedProject));
        MessageBus.Current.SendMessage(new LanguageCodeChangedMessage(SelectedLanguage));
        dialog.Close(true);
    }
}

public class Alerts
{
    [Reactive]
    public bool AmbiguousCitationDates { get; set; }
    
    [Reactive]
    public bool ContainsSicTag { get; set; }
    
    [Reactive]
    public bool DabPageWithRef { get; set; }
    
    [Reactive]
    public bool DeadLinks { get; set; }
    
    [Reactive]
    public bool DuplicateParametersInWpBannerShell { get; set; }
    
    [Reactive]
    public bool HasRefAfterReferences { get; set; }
    
    [Reactive]
    public bool HasFootnotesTemplate { get; set; }
    
    [Reactive]
    public bool HeadersWithWikilinks { get; set; }
    
    [Reactive]
    public bool InvalidCitationParameters { get; set; }
    
    [Reactive]
    public bool LinksWithDoublePipes { get; set; }
    
    [Reactive]
    public bool LinksWithNoTarget { get; set; }
    
    [Reactive]
    public bool LongArticleWithStubTag { get; set; }
    
    [Reactive]
    public bool MultipleDefaultSort { get; set; }
    
    [Reactive]
    public bool NoCategory { get; set; }
    
    [Reactive]
    public bool SeeAlsoOutOfPlace { get; set; }
    
    [Reactive]
    public bool StartsWithHeading { get; set; }
    
    [Reactive]
    public bool UnbalancedBrackets { get; set; }
    
    [Reactive]
    public bool UnclosedTags { get; set; }
    
    [Reactive]
    public bool UnformattedReferences { get; set; }
    
    [Reactive]
    public bool UnknownParametersInMultipleIssues { get; set; }
    
    [Reactive]
    public bool UnknownParametersInWpBannerShell { get; set; }
    
    [Reactive]
    public bool EditorsSignatureOrLink { get; set; }
}
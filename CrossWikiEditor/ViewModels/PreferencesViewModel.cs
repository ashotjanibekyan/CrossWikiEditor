using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public class PreferencesViewModel : ViewModelBase
{
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

    [Reactive] 
    public string SelectedProject { get; set; } = "wikipedia";

    [Reactive]
    public ObservableCollection<string> Projects { get; set; } = new(new List<string>
    {
        "wikipedia",
        "wikiquote",
        "wiktionary"
    });

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
}
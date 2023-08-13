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
}
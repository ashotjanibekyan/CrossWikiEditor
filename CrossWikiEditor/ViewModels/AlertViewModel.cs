using System.Reactive;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public sealed class AlertViewModel : ViewModelBase
{
    public AlertViewModel()
    {
        OkCommand = ReactiveCommand.Create((IDialog dialog) => dialog.Close(false));
    }

    public string ContentText { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ReactiveCommand<IDialog, Unit> OkCommand { get; }
}
using System.Reactive;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public sealed class AlertViewModel : ViewModelBase
{
    public AlertViewModel(string title, string contentText)
    {
        Title = title;
        ContentText = contentText;
        OkCommand = ReactiveCommand.Create((IDialog dialog) => dialog.Close(false));
    }

    public string ContentText { get; }
    public string Title { get; }
    public ReactiveCommand<IDialog, Unit> OkCommand { get; }
}
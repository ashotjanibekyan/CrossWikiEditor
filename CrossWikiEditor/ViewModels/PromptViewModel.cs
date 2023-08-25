using System.Reactive;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class PromptViewModel : ViewModelBase
{
    public PromptViewModel(string title, string text)
    {
        Title = title;
        Text = text;
        
        OkCommand = ReactiveCommand.Create((IDialog dialog) => dialog.Close(Value));
        CancelCommand = ReactiveCommand.Create((IDialog dialog) => dialog.Close(-1));
    }

    public bool IsNumeric { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public int Value { get; set; }
    public ReactiveCommand<IDialog, Unit> OkCommand { get; }
    public ReactiveCommand<IDialog, Unit> CancelCommand { get; }
}
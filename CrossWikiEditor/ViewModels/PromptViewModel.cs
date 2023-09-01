using CommunityToolkit.Mvvm.Input;

namespace CrossWikiEditor.ViewModels;

public sealed partial class PromptViewModel(string title, string text) : ViewModelBase
{
    public bool IsNumeric { get; set; }
    public string Title { get; set; } = title;
    public string Text { get; set; } = text;
    public int Value { get; set; }

    [RelayCommand]
    private void Ok(IDialog dialog)
    {
        dialog.Close(Value);
    }

    [RelayCommand]
    private void Cancel(IDialog dialog)
    {
        dialog.Close(null);
    }
}
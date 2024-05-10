namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class PromptViewModel(string title, string text) : ViewModelBase
{
    public required bool IsNumeric { get; init; }
    public string Title { get; } = title;
    public string Text { get; } = text;
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
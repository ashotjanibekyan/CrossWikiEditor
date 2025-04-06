using CommunityToolkit.Mvvm.Input;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class PromptViewModel : ViewModelBase
{
    public PromptViewModel(string title, string text)
    {
        Title = title;
        Text = text;
    }

    public required bool IsNumeric { get; init; }
    public string Title { get; }
    public string Text { get; }
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
using CommunityToolkit.Mvvm.Input;

namespace CrossWikiEditor.ViewModels;

public sealed partial class AlertViewModel(string title, string contentText) : ViewModelBase
{
    public string ContentText { get; } = contentText;
    public string Title { get; } = title;

    [RelayCommand]
    private void Ok(IDialog dialog)
    {
        dialog.Close(false);
    }
}
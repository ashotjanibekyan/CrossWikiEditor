using CommunityToolkit.Mvvm.Input;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class AlertViewModel : ViewModelBase
{
    public AlertViewModel(string title, string contentText)
    {
        ContentText = contentText;
        Title = title;
    }

    public string ContentText { get; }
    public string Title { get; }

    [RelayCommand]
    private void Ok(IDialog dialog)
    {
        dialog.Close(false);
    }
}
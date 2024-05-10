namespace CrossWikiEditor.Core.ViewModels.ReportViewModels;

public sealed partial class EditBoxViewModel : ViewModelBase
{
    public EditBoxViewModel(IMessengerWrapper messenger)
    {
        messenger.Register<PageUpdatingMessage>(this, (recipient, message) =>
        {
            Content = message.NewContent;
        });
    }
    
    [ObservableProperty] private string _content = string.Empty;
}
using CommunityToolkit.Mvvm.ComponentModel;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels.ReportViewModels;

public sealed partial class EditBoxViewModel : ViewModelBase
{
    public EditBoxViewModel(IMessengerWrapper messenger)
    {
        Content = string.Empty;
        messenger.Register<PageUpdatingMessage>(this, (recipient, message) => Content = message.NewContent);
    }

    [ObservableProperty] public partial string Content { get; set; }
}
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Messages.PageProcessingMessages;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class StartViewModel : ViewModelBase
{
    private readonly IMessengerWrapper _messenger;
    private bool _isProcessing;
    private bool _isSaving;

    public StartViewModel(IMessengerWrapper messenger)
    {
        _messenger = messenger;
        _messenger.Register<PageProcessingMessage>(this, (_, _) => _isProcessing = true);
        _messenger.Register<PageProcessedMessage>(this, (_, _) => _isProcessing = false);
        _messenger.Register<PageSavingMessage>(this, (_, _) => _isSaving = true);
        _messenger.Register<PageSavedMessage>(this, (_, _) => _isSaving = false);
    }

    private bool IsBusy => _isProcessing || _isSaving;

    public int WordsStats { get; set; }
    public int LinksStats { get; set; }
    public int ImagesStats { get; set; }
    public int CategoriesStats { get; set; }
    public int InterwikiLinksStats { get; set; }
    public int DatesStats { get; set; }

    [RelayCommand]
    private void Start()
    {
        _messenger.Send(new StartBotMessage());
    }

    [RelayCommand]
    private void Stop()
    {
        _messenger.Send(new StopBotMessage());
    }

    [RelayCommand]
    private void Save()
    {
        if (!IsBusy)
        {
            _messenger.Send(new SaveOrSkipPageMessage(true));
        }
    }

    [RelayCommand]
    private void Skip()
    {
        if (!IsBusy)
        {
            _messenger.Send(new SaveOrSkipPageMessage(false));
        }
    }
}

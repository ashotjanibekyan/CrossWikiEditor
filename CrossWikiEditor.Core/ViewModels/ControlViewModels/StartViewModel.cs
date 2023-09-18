namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class StartViewModel(IMessengerWrapper messenger) : ViewModelBase
{
    public int WordsStats { get; set; }
    public int LinksStats { get; set; }
    public int ImagesStats { get; set; }
    public int CategoriesStats { get; set; }
    public int InterwikiLinksStats { get; set; }
    public int DatesStats { get; set; }

    [RelayCommand]
    private void Start()
    {
        messenger.Send(new StartBotMessage());
    }

    [RelayCommand]
    private void Stop()
    {
        messenger.Send(new StopBotMessage());
    }
}
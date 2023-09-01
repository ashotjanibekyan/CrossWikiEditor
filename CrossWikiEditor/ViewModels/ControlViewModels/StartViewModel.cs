using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ViewModels.ControlViewModels;

public sealed partial class StartViewModel(IMessengerWrapper messenger) : ViewModelBase
{
    public IEnumerable<string> Summaries { get; set; } = new List<string>
    {
        "Summary 1",
        "Summary 2",
        "Summary 3",
        "Summary 4",
        "Summary 5",
        "Summary 6",
        "Summary 7",
        "Summary 8",
        "Summary 9",
        "Summary 10"
    };

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
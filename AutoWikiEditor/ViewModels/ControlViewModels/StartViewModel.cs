using System.Collections.Generic;

namespace AutoWikiEditor.ViewModels.ControlViewModels;

public class StartViewModel : ViewModelBase
{
    public StartViewModel()
    {
        this.Summaries = new List<string>
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
            "Summary 10",
        };
    }

    public IEnumerable<string> Summaries { get; set; }

    public int WordsStats { get; set; }
    public int LinksStats { get; set; }
    public int ImagesStats { get; set; }
    public int CategoriesStats { get; set; }
    public int InterwikiLinksStats { get; set; }
    public int DatesStats { get; set; }
}

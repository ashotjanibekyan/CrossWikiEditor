using System.Collections.Generic;

namespace CrossWikiEditor.ViewModels;

public sealed class MakeListViewModel : ViewModelBase
{

    public MakeListViewModel()
    {
        this.Sources = new List<string>
        {
            "List 1",
            "List 2",
            "List 3",
            "List 4"
        };
        this.Pages = new List<string>
        {
            "Page 1",
            "Page 2",
            "Page 3",
            "Page 4",
            "Page 5",
            "Page 6",
            "Page 7",
            "Page 8",
            "Page 9",
            "Page 10",
            "Page 11",
            "Page 12",
            "Page 13",
            "Page 14",
            "Page 15",
            "Page 16",
            "Page 17",
            "Page 18",
            "Page 19",
            "Page 20",
        };
    }

    public IEnumerable<string> Sources { get; }
    public IEnumerable<string> Pages { get; }
}
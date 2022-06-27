namespace AutoWikiEditor.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        this.MenuViewModel = new MenuViewModel();
        this.MakeListViewModel = new MakeListViewModel();
    }

    public MenuViewModel MenuViewModel { get; set; }
    public MakeListViewModel MakeListViewModel { get; set; }
}
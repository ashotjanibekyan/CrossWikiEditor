using AutoWikiEditor.ViewModels.ControlViewModels;

namespace AutoWikiEditor.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        this.DisambigViewModel = new DisambigViewModel();
        this.MoreViewModel = new MoreViewModel();
        this.OptionsViewModel = new OptionsViewModel();
        this.SkipViewModel = new SkipViewModel();
        this.StartViewModel = new StartViewModel();
        this.MakeListViewModel = new MakeListViewModel();
        this.MenuViewModel = new MenuViewModel();
    }

    public DisambigViewModel DisambigViewModel { get; set; }
    public MoreViewModel MoreViewModel { get; set; }
    public OptionsViewModel OptionsViewModel { get; set; }
    public SkipViewModel SkipViewModel { get; set; }
    public StartViewModel StartViewModel { get; set; }
    public MakeListViewModel MakeListViewModel { get; set; }
    public MenuViewModel MenuViewModel { get; set; }
}
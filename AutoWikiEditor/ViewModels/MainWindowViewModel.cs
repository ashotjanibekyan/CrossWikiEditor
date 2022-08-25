using AutoWikiEditor.ViewModels.ControlViewModels;
using AutoWikiEditor.ViewModels.ReportViewModels;

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

        this.EditBoxViewModel  = new EditBoxViewModel();
        this.HistoryViewModel  = new HistoryViewModel();
        this.WhatLinksHereViewModel  = new WhatLinksHereViewModel();
        this.LogsViewModel  = new LogsViewModel();
        this.PageLogsViewModel = new PageLogsViewModel();

        this.StatusBarViewModel = new StatusBarViewModel();
    }
    
    public DisambigViewModel DisambigViewModel { get; set; }
    public MoreViewModel MoreViewModel { get; set; }
    public OptionsViewModel OptionsViewModel { get; set; }
    public SkipViewModel SkipViewModel { get; set; }
    public StartViewModel StartViewModel { get; set; }
    public MakeListViewModel MakeListViewModel { get; set; }
    public MenuViewModel MenuViewModel { get; set; }
    public EditBoxViewModel EditBoxViewModel { get; set; }
    public HistoryViewModel HistoryViewModel { get; set; }
    public WhatLinksHereViewModel WhatLinksHereViewModel { get; set; }
    public LogsViewModel LogsViewModel { get; set; }
    public PageLogsViewModel PageLogsViewModel { get; set; }
    public StatusBarViewModel StatusBarViewModel { get; set; }
}
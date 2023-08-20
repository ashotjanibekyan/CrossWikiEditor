using CrossWikiEditor.ViewModels.ControlViewModels;
using CrossWikiEditor.ViewModels.ReportViewModels;

namespace CrossWikiEditor.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    public StatusBarViewModel StatusBarViewModel { get; }
    
    public MakeListViewModel MakeListViewModel { get; }
    public OptionsViewModel OptionsViewModel { get; }
    public MoreViewModel MoreViewModel { get; }
    public DisambigViewModel DisambigViewModel { get; }
    public SkipViewModel SkipViewModel { get; }
    public StartViewModel StartViewModel { get; }
    
    
    public EditBoxViewModel EditBoxViewModel { get; }
    public HistoryViewModel HistoryViewModel { get; }
    public WhatLinksHereViewModel WhatLinksHereViewModel { get; }
    public LogsViewModel LogsViewModel { get; }
    public PageLogsViewModel PageLogsViewModel { get; }
    

    public MainWindowViewModel(
        StatusBarViewModel statusBarViewModel, 
        MakeListViewModel makeListViewModel, 
        OptionsViewModel optionsViewModel, 
        MoreViewModel moreViewModel,
        DisambigViewModel disambigViewModel, 
        SkipViewModel skipViewModel, 
        StartViewModel startViewModel, 
        EditBoxViewModel editBoxViewModel, 
        HistoryViewModel historyViewModel, 
        WhatLinksHereViewModel whatLinksHereViewModel, 
        LogsViewModel logsViewModel, 
        PageLogsViewModel pageLogsViewModel)
    {
        StatusBarViewModel = statusBarViewModel;
        MakeListViewModel = makeListViewModel;
        OptionsViewModel = optionsViewModel;
        MoreViewModel = moreViewModel;
        DisambigViewModel = disambigViewModel;
        SkipViewModel = skipViewModel;
        StartViewModel = startViewModel;
        EditBoxViewModel = editBoxViewModel;
        HistoryViewModel = historyViewModel;
        WhatLinksHereViewModel = whatLinksHereViewModel;
        LogsViewModel = logsViewModel;
        PageLogsViewModel = pageLogsViewModel;
    }
}
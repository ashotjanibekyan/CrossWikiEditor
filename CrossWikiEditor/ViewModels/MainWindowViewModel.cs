using CrossWikiEditor.ViewModels.ControlViewModels;
using CrossWikiEditor.ViewModels.MenuViewModels;
using CrossWikiEditor.ViewModels.ReportViewModels;

namespace CrossWikiEditor.ViewModels;

public sealed class MainWindowViewModel(StatusBarViewModel statusBarViewModel,
        MakeListViewModel makeListViewModel,
        OptionsViewModel optionsViewModel,
        MoreViewModel moreViewModel,
        MenuViewModel menuViewModel,
        DisambigViewModel disambigViewModel,
        SkipViewModel skipViewModel,
        StartViewModel startViewModel,
        EditBoxViewModel editBoxViewModel,
        HistoryViewModel historyViewModel,
        WhatLinksHereViewModel whatLinksHereViewModel,
        LogsViewModel logsViewModel,
        PageLogsViewModel pageLogsViewModel)
    : ViewModelBase
{
    public StatusBarViewModel StatusBarViewModel { get; } = statusBarViewModel;

    public MakeListViewModel MakeListViewModel { get; } = makeListViewModel;
    public OptionsViewModel OptionsViewModel { get; } = optionsViewModel;
    public MoreViewModel MoreViewModel { get; } = moreViewModel;
    public MenuViewModel MenuViewModel { get; set; } = menuViewModel;
    public DisambigViewModel DisambigViewModel { get; } = disambigViewModel;
    public SkipViewModel SkipViewModel { get; } = skipViewModel;
    public StartViewModel StartViewModel { get; } = startViewModel;


    public EditBoxViewModel EditBoxViewModel { get; } = editBoxViewModel;
    public HistoryViewModel HistoryViewModel { get; } = historyViewModel;
    public WhatLinksHereViewModel WhatLinksHereViewModel { get; } = whatLinksHereViewModel;
    public LogsViewModel LogsViewModel { get; } = logsViewModel;
    public PageLogsViewModel PageLogsViewModel { get; } = pageLogsViewModel;
}
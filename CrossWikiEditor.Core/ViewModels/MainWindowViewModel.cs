using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.ViewModels.ControlViewModels;
using CrossWikiEditor.Core.ViewModels.MenuViewModels;
using CrossWikiEditor.Core.ViewModels.ReportViewModels;

namespace CrossWikiEditor.Core.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private Task? _myBot;
    public MainWindowViewModel(StatusBarViewModel statusBarViewModel,
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
        PageLogsViewModel pageLogsViewModel,
        IMessengerWrapper messenger)
    {
        StatusBarViewModel = statusBarViewModel;
        MakeListViewModel = makeListViewModel;
        OptionsViewModel = optionsViewModel;
        MoreViewModel = moreViewModel;
        MenuViewModel = menuViewModel;
        DisambigViewModel = disambigViewModel;
        SkipViewModel = skipViewModel;
        StartViewModel = startViewModel;
        EditBoxViewModel = editBoxViewModel;
        HistoryViewModel = historyViewModel;
        WhatLinksHereViewModel = whatLinksHereViewModel;
        LogsViewModel = logsViewModel;
        PageLogsViewModel = pageLogsViewModel;
        messenger.Register<StartBotMessage>(this, (recipient, message) =>
        {
            _myBot = Task.Run((Func<Task?>) (async () =>
            {
                var pageProcessor = new PageListProcessor(messenger, Enumerable.ToList<WikiPageModel>(MakeListViewModel.Pages), OptionsViewModel.NormalFindAndReplaceRules);
                await pageProcessor.Start();
            }));
        });
    }

    public StatusBarViewModel StatusBarViewModel { get; }
    public MakeListViewModel MakeListViewModel { get; }
    public OptionsViewModel OptionsViewModel { get; }
    public MoreViewModel MoreViewModel { get; }
    public MenuViewModel MenuViewModel { get; set; }
    public DisambigViewModel DisambigViewModel { get; }
    public SkipViewModel SkipViewModel { get; }
    public StartViewModel StartViewModel { get; }
    public EditBoxViewModel EditBoxViewModel { get; }
    public HistoryViewModel HistoryViewModel { get; }
    public WhatLinksHereViewModel WhatLinksHereViewModel { get; }
    public LogsViewModel LogsViewModel { get; }
    public PageLogsViewModel PageLogsViewModel { get; }
}
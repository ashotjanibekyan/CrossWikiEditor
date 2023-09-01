using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Utils;
using CrossWikiEditor.ViewModels.ControlViewModels;
using CrossWikiEditor.ViewModels.MenuViewModels;
using CrossWikiEditor.ViewModels.ReportViewModels;

namespace CrossWikiEditor.ViewModels;

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
            _myBot = Task.Run(async () =>
            {
                var pageProcessor = new PageListProcessor(messenger, MakeListViewModel.Pages.ToList(), OptionsViewModel.NormalFindAndReplaceRules);
                await pageProcessor.Start();
            });
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
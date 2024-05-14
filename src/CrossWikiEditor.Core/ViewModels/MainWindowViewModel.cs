namespace CrossWikiEditor.Core.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private PageListProcessor? _listProcessor;
    private Task? _myBot;

    public static MainWindowViewModel? Instance { get; private set; }

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
        IMessengerWrapper messenger,
        ISettingsService settingsService)
    {
        Instance = this;
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
                _listProcessor?.Stop();
                _listProcessor = new PageListProcessor(messenger, settingsService, [..MakeListViewModel.Pages], OptionsViewModel.NormalFindAndReplaceRules);
                await _listProcessor.Start();
            });
        });
        messenger.Register<StopBotMessage>(this, (recipient, message) => _listProcessor?.Stop());
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
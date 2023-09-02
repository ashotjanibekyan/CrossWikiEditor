using System;
using System.IO;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using CrossWikiEditor.ViewModels;
using CrossWikiEditor.ViewModels.ControlViewModels;
using CrossWikiEditor.ViewModels.MenuViewModels;
using CrossWikiEditor.ViewModels.ReportViewModels;
using CrossWikiEditor.Views;
using CrossWikiEditor.Views.ControlViews;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace CrossWikiEditor;

public class App : Application
{
    private IContainer? _container;
    private MainWindow? _mainWindow;
    private string _appData;

    public App()
    {
        _appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrossWikiBrowser");
        if (!Directory.Exists(_appData))
        {
            Directory.CreateDirectory(_appData);
        }
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainWindow = new MainWindow();
            _container = BuildDiContainer();

            _mainWindow.DataContext = _container.Resolve<MainWindowViewModel>();

            desktop.MainWindow = _mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private IContainer BuildDiContainer()
    {
        var builder = new ContainerBuilder();

        RegisterViewModels(builder);
        RegisterServices(builder);
        RegisterRepositories(builder);
        RegisterDialogs(builder);
        RegisterListProviders(builder);
        RegisterUtils(builder);

        builder.Register(c => _container!).As<IContainer>();

        return builder.Build();
    }

    private void RegisterRepositories(ContainerBuilder builder)
    {
        builder.RegisterType<SimpleJsonProfileRepository>().As<IProfileRepository>();
    }

    private void RegisterServices(ContainerBuilder builder)
    {
        IStorageProvider storageProvider = TopLevel.GetTopLevel(_mainWindow)!.StorageProvider;
        (byte[] key, byte[] iv) = StringEncryptionService.GenerateKeyAndIv("SHOULD IMPLEMENT THIS LATER");
        IStringEncryptionService stringEncryptionService = new StringEncryptionService(key, iv);

        Logger logger = new LoggerConfiguration()
            .WriteTo.Async(a => a.File(new JsonFormatter(), "log.json"))
#if DEBUG
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Information()
#endif
            .CreateLogger();

        builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>().SingleInstance();
        builder.RegisterType<FileDialogService>().As<IFileDialogService>().SingleInstance();
        builder.RegisterType<SystemService>().As<ISystemService>().SingleInstance();
        builder.RegisterType<UserPreferencesService>().As<IUserPreferencesService>().SingleInstance();

        builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
        builder.RegisterType<PageService>().As<IPageService>().SingleInstance();
        builder.RegisterType<WikiClientCache>().As<IWikiClientCache>().SingleInstance();

        builder.RegisterType<DialogService>()
            .As<IDialogService>()
            .WithParameter(new TypedParameter(typeof(IOwner), _mainWindow)).SingleInstance();
        builder.RegisterInstance(storageProvider).As<IStorageProvider>();
        builder.RegisterInstance(stringEncryptionService).As<IStringEncryptionService>();
        builder.RegisterInstance(logger).As<ILogger>();
        builder.Register(c => TopLevel.GetTopLevel(_mainWindow)?.Clipboard).As<IClipboard>();
        builder.RegisterInstance(new MessengerWrapper(WeakReferenceMessenger.Default)).As<IMessengerWrapper>();
    }

    private void RegisterViewModels(ContainerBuilder builder)
    {
        builder.RegisterType<MainWindowViewModel>();
        builder.RegisterType<StatusBarViewModel>();

        builder.RegisterType<MakeListViewModel>();
        builder.RegisterType<OptionsViewModel>();
        builder.RegisterType<MoreViewModel>();
        builder.RegisterType<MenuViewModel>();
        builder.RegisterType<FileMenuViewModel>().WithParameter(new TypedParameter(typeof(Window), _mainWindow));
        builder.RegisterType<DisambigViewModel>();
        builder.RegisterType<SkipViewModel>();
        builder.RegisterType<StartViewModel>();

        builder.RegisterType<EditBoxViewModel>();
        builder.RegisterType<HistoryViewModel>();
        builder.RegisterType<WhatLinksHereViewModel>();
        builder.RegisterType<LogsViewModel>();
        builder.RegisterType<PageLogsViewModel>();
    }

    private void RegisterDialogs(ContainerBuilder builder)
    {
        builder.RegisterType<AlertView>().Named<IDialog>(nameof(AlertViewModel));
        builder.RegisterType<PromptView>().Named<IDialog>(nameof(PromptViewModel));
        builder.RegisterType<FilterView>().Named<IDialog>(nameof(FilterViewModel));
        builder.RegisterType<ProfilesView>().Named<IDialog>(nameof(ProfilesViewModel));
        builder.RegisterType<PreferencesView>().Named<IDialog>(nameof(PreferencesViewModel));
        builder.RegisterType<AddNewProfileView>().Named<IDialog>(nameof(AddOrEditProfileViewModel));
        builder.RegisterType<SelectNamespacesView>().Named<IDialog>(nameof(SelectNamespacesViewModel));
        builder.RegisterType<WhatLinksHereOptionsView>().Named<IDialog>(nameof(WhatLinksHereOptionsViewModel));
        builder.RegisterType<FindAndReplaceView>().Named<IDialog>(nameof(FindAndReplaceViewModel));
    }

    private void RegisterListProviders(ContainerBuilder builder)
    {
        builder.RegisterType<CategoriesOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<CategoriesOnPageNoHiddenCategoriesListProvider>().As<IListProvider>();
        builder.RegisterType<CategoriesOnPageOnlyHiddenCategoriesListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryRecursive1LevelListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryRecursiveUserDefinedLevelListProviderBase>().As<IListProvider>();
        builder.RegisterType<CategoryRecursiveListProvider>().As<IListProvider>();
        // TODO: CheckWiki error
        // TODO: CheckWiki error (number)
        // TODO: Database dump
        builder.RegisterType<FilesOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<GoogleSearchListProvider>().As<IListProvider>();
        builder.RegisterType<HtmlScraperListProvider>().As<IListProvider>();
        builder.RegisterType<ImageFileLinksListProvider>().As<IListProvider>();
        builder.RegisterType<LinksOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<LinksOnPageBlueListProvider>().As<IListProvider>();
        builder.RegisterType<LinksOnPageRedListProvider>().As<IListProvider>();
        builder.RegisterType<MyWatchlistListProvider>().As<IListProvider>();
        builder.RegisterType<NewPagesListProvider>().As<IListProvider>();
        builder.RegisterType<PagesWithPropListProvider>().As<IListProvider>();
        builder.RegisterType<RandomListProvider>().As<IListProvider>();
        // todo: Special pages
        builder.RegisterType<TextFileListProvider>().As<IListProvider>();
        builder.RegisterType<TransclusionsOnPageListProvider>().As<IListProvider>();
        //builder.RegisterType<UserContribsListProvider>().As<IListProvider>();
        builder.RegisterType<WhatLinksHereListProvider>().As<IListProvider>();
        builder.RegisterType<WhatTranscludesHereListProvider>().As<IListProvider>();
        builder.RegisterType<WhatTranscludesHereAllNsListProvider>().As<IListProvider>();
        builder.RegisterType<WikiSearchInTitleListProvider>().As<IListProvider>();
        builder.RegisterType<WikiSearchInTitleAllNsListProvider>().As<IListProvider>();
        builder.RegisterType<WikiSearchInTextListProvider>().As<IListProvider>();
        builder.RegisterType<WikiSearchInTextAllNsListProvider>().As<IListProvider>();

        builder.RegisterType<TextFileListProvider>();
    }

    private void RegisterUtils(ContainerBuilder builder)
    {
        builder.RegisterType<LanguageSpecificRegexes>();
    }
}
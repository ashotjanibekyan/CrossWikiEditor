using System;
using System.IO;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.ViewModels;
using CrossWikiEditor.ViewModels.ControlViewModels;
using CrossWikiEditor.ViewModels.MenuViewModels;
using CrossWikiEditor.ViewModels.ReportViewModels;
using CrossWikiEditor.Views;
using ReactiveUI;

namespace CrossWikiEditor;

public class App : Application
{
    private IContainer? _container;
    private Window? _mainWindow;
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

        builder.Register(c => _container!).As<IContainer>();

        return builder.Build();
    }

    private void RegisterRepositories(ContainerBuilder builder)
    {
        builder.RegisterType<RealmProfileRepository>()
            .As<IProfileRepository>()
            .WithParameter(new PositionalParameter(0, _appData)).SingleInstance();
    }

    private void RegisterServices(ContainerBuilder builder)
    {
        IStorageProvider storageProvider = TopLevel.GetTopLevel(_mainWindow)!.StorageProvider;
        (byte[] key, byte[] iv) = StringEncryptionService.GenerateKeyAndIv("SHOULD IMPLEMENT THIS LATER");
        IStringEncryptionService stringEncryptionService = new StringEncryptionService(key, iv);

        builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>().SingleInstance();
        builder.RegisterType<FileDialogService>().As<IFileDialogService>().SingleInstance();
        builder.RegisterType<SystemService>().As<ISystemService>().SingleInstance();
        builder.RegisterType<UserPreferencesService>().As<IUserPreferencesService>().SingleInstance();
        
        builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
        builder.RegisterType<PageService>().As<IPageService>().SingleInstance();
        builder.RegisterType<WikiClientCache>().As<IWikiClientCache>().SingleInstance();
        
        builder.RegisterType<DialogService>()
            .As<IDialogService>()
            .WithParameter(new TypedParameter(typeof(Window), _mainWindow)).SingleInstance();
        builder.RegisterInstance(storageProvider).As<IStorageProvider>();
        builder.RegisterInstance(stringEncryptionService).As<IStringEncryptionService>();
        builder.Register(c => MessageBus.Current).As<IMessageBus>();
        builder.Register(c => TopLevel.GetTopLevel(_mainWindow)?.Clipboard).As<IClipboard>();
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
        builder.RegisterType<AlertView>().Named<Window>(nameof(AlertViewModel));
        builder.RegisterType<PromptView>().Named<Window>(nameof(PromptViewModel));
        builder.RegisterType<FilterView>().Named<Window>(nameof(FilterViewModel));
        builder.RegisterType<ProfilesView>().Named<Window>(nameof(ProfilesViewModel));
        builder.RegisterType<PreferencesView>().Named<Window>(nameof(PreferencesViewModel));
        builder.RegisterType<AddNewProfileView>().Named<Window>(nameof(AddOrEditProfileViewModel));
    }

    private void RegisterListProviders(ContainerBuilder builder)
    {
        builder.RegisterType<CategoriesOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<CategoriesOnPageNoHiddenCategoriesListProvider>().As<IListProvider>();
        builder.RegisterType<CategoriesOnPageOnlyHiddenCategoriesListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryRecursive1LevelListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryRecursiveUserDefinedLevelListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryRecursiveListProvider>().As<IListProvider>();
        builder.RegisterType<FilesOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<TextFileListProvider>().As<IListProvider>();
        
        builder.RegisterType<TextFileListProvider>();
    }
}
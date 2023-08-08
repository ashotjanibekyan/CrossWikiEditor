using System;
using System.IO;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.ViewModels;
using CrossWikiEditor.ViewModels.ControlViewModels;
using CrossWikiEditor.ViewModels.ReportViewModels;
using CrossWikiEditor.Views;

namespace CrossWikiEditor;

public class App : Application
{
    private IContainer? _container;
    private Window? _mainWindow;
    private string _connectionString;

    public App()
    {
        var appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrossWikiBrowser");
        if (!Directory.Exists(appdata))
        {
            Directory.CreateDirectory(appdata);
        }
        _connectionString = $"Data Source={Path.Combine(appdata, "cwb.db")};Version=3;";
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
        
        builder.Register(c => _container!).As<IContainer>();

        return builder.Build();
    }

    private void RegisterRepositories(ContainerBuilder builder)
    {
        builder.RegisterType<ProfileRepository>()
            .As<IProfileRepository>()
            .WithParameter(new PositionalParameter(0, _connectionString)).SingleInstance();
    }

    private void RegisterServices(ContainerBuilder builder)
    {
        var storageProvider = TopLevel.GetTopLevel(_mainWindow)!.StorageProvider;
        builder.RegisterInstance(storageProvider).As<IStorageProvider>();
        builder.RegisterType<DialogService>()
            .As<IDialogService>()
            .WithParameter(new TypedParameter(typeof(Window), _mainWindow)).SingleInstance();
        builder.RegisterType<CredentialService>().As<ICredentialService>().SingleInstance();
        builder.RegisterType<FileDialogService>().As<IFileDialogService>().SingleInstance();
        var (key, iv) = StringEncryptionService.GenerateKeyAndIv("SHOULD IMPLEMENT THIS LATER", new byte[16], 32, 16, 10000);
        IStringEncryptionService stringEncryptionService = new StringEncryptionService(key, iv);
        builder.RegisterInstance(stringEncryptionService).As<IStringEncryptionService>();
    }
    
    private void RegisterViewModels(ContainerBuilder builder)
    {
        builder.RegisterType<MainWindowViewModel>();
        builder.RegisterType<StatusBarViewModel>();

        builder.RegisterType<MakeListViewModel>();
        builder.RegisterType<OptionsViewModel>();
        builder.RegisterType<MoreViewModel>();
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
        builder.RegisterType<ProfilesView>().Named<Window>(nameof(ProfilesViewModel));
        builder.RegisterType<AddNewProfileView>().Named<Window>(nameof(AddOrEditProfileViewModel));
    }
}
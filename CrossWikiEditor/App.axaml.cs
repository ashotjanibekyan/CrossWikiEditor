using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
        
        builder.RegisterType<ProfilesView>().Named<Window>(nameof(ProfilesViewModel));
        builder.Register(c => _container!).As<IContainer>();

        return builder.Build();
    }

    private void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<DialogService>()
            .As<IDialogService>()
            .WithParameter(new TypedParameter(typeof(Window), _mainWindow)).SingleInstance();
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
}
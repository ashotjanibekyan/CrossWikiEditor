using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrossWikiEditor.ViewModels;
using CrossWikiEditor.ViewModels.ControlViewModels;
using CrossWikiEditor.ViewModels.ReportViewModels;
using CrossWikiEditor.Views;

namespace CrossWikiEditor;

public class App : Application
{
    private IContainer? _container;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _container = BuildDiContainer();

            desktop.MainWindow = new MainWindow
            {
                DataContext = _container.Resolve<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private IContainer BuildDiContainer()
    {
        var builder = new ContainerBuilder();

        RegisterViewModels(builder);

        return builder.Build();
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
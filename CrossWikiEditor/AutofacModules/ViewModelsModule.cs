using Autofac;
using Avalonia.Controls;
using CrossWikiEditor.Core.ViewModels;
using CrossWikiEditor.Core.ViewModels.ControlViewModels;
using CrossWikiEditor.Core.ViewModels.MenuViewModels;
using CrossWikiEditor.Core.ViewModels.ReportViewModels;
using CrossWikiEditor.Views;

namespace CrossWikiEditor.AutofacModules;

public sealed class ViewModelsModule(MainWindow mainWindow) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MainWindowViewModel>();
        builder.RegisterType<StatusBarViewModel>();

        builder.RegisterType<MakeListViewModel>();
        builder.RegisterType<OptionsViewModel>();
        builder.RegisterType<MoreViewModel>();
        builder.RegisterType<MenuViewModel>();
        builder.RegisterType<FileMenuViewModel>().WithParameter(new TypedParameter(typeof(Window), mainWindow));
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
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
        builder.RegisterType<DisambigViewModel>();
        builder.RegisterType<EditBoxViewModel>();
        builder.RegisterType<FileMenuViewModel>().WithParameter(new TypedParameter(typeof(Window), mainWindow));
        builder.RegisterType<HistoryViewModel>();
        builder.RegisterType<LogsViewModel>();
        builder.RegisterType<MainWindowViewModel>();
        builder.RegisterType<MakeListViewModel>();
        builder.RegisterType<MenuViewModel>();
        builder.RegisterType<MoreViewModel>();
        builder.RegisterType<OptionsViewModel>();
        builder.RegisterType<PageLogsViewModel>();
        builder.RegisterType<SkipViewModel>();
        builder.RegisterType<StartViewModel>();
        builder.RegisterType<StatusBarViewModel>();
        builder.RegisterType<WhatLinksHereViewModel>();
    }
}
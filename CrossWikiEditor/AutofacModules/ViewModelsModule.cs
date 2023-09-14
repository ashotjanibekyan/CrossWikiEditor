using CrossWikiEditor.Core.ViewModels;
using CrossWikiEditor.Core.ViewModels.ControlViewModels;
using CrossWikiEditor.Core.ViewModels.MenuViewModels;
using CrossWikiEditor.Core.ViewModels.ReportViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CrossWikiEditor.AutofacModules;

public static class ViewModelsModule
{
    public static void Register(IServiceCollection services)
    {
        services.AddTransient<DisambigViewModel>();
        services.AddTransient<EditBoxViewModel>();
        services.AddTransient<FileMenuViewModel>();
        services.AddTransient<HistoryViewModel>();
        services.AddTransient<LogsViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MakeListViewModel>();
        services.AddTransient<MenuViewModel>();
        services.AddTransient<MoreViewModel>();
        services.AddTransient<OptionsViewModel>();
        services.AddTransient<PageLogsViewModel>();
        services.AddTransient<SkipViewModel>();
        services.AddTransient<StartViewModel>();
        services.AddTransient<StatusBarViewModel>();
        services.AddTransient<WhatLinksHereViewModel>();
    }
}
using System;
using System.IO;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrossWikiEditor.AutofacModules;
using CrossWikiEditor.Core.Repositories;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.ViewModels;
using CrossWikiEditor.Views;

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
        builder.RegisterModule(new ServicesModule(_mainWindow!));
        builder.RegisterModule(new ViewModelsModule(_mainWindow!));
        builder.RegisterModule<DialogsModule>();
        builder.RegisterModule<ListProvidersModule>();

        builder.RegisterType<SimpleJsonProfileRepository>().As<IProfileRepository>();
        builder.RegisterType<LanguageSpecificRegexes>();
        builder.Register(c => _container!).As<IContainer>();
        return builder.Build();
    }
}
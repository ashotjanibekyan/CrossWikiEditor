using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using CrossWikiEditor.Core;
using CrossWikiEditor.Core.Repositories;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.ViewModels;
using CrossWikiEditor.DependencyModules;
using CrossWikiEditor.Views;

using Microsoft.Extensions.DependencyInjection;

namespace CrossWikiEditor;

public sealed class App : Application
{
    private MainWindow? _mainWindow;

    public App()
    {
        string? appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrossWikiBrowser");
        if (!Directory.Exists(appData))
        {
            Directory.CreateDirectory(appData);
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
            var services = new ServiceCollection();
            services.AddKeyedTransient<IDialog, AddNewProfileView>(nameof(AddOrEditProfileViewModel));

            DialogsModule.Register(services);
            ListProvidersModule.Register(services);
            ViewModelsModule.Register(services);
            ServicesModule.Register(services, _mainWindow);
            services.AddSingleton<IProfileRepository, SimpleJsonProfileRepository>();
            services.AddSingleton<LanguageSpecificRegexes>();
            services.AddHttpClient();
            ServiceProvider sp = services.BuildServiceProvider();
            _mainWindow.DataContext = sp.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = _mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
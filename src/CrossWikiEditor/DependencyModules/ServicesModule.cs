using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.HtmlParsers;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Services;
using CrossWikiEditor.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace CrossWikiEditor.DependencyModules;

public static class ServicesModule
{
    public static void Register(IServiceCollection services, MainWindow mainWindow)
    {
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

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<ICategoryService, CategoryService>();
        services.AddSingleton<IWikiClientCache, WikiClientCache>();
        services.AddSingleton<IDialogService, DialogService>(sp => new DialogService(sp, mainWindow));
        services.AddTransient(_ => TopLevel.GetTopLevel(mainWindow)!.StorageProvider);
        services.AddTransient(_ => TopLevel.GetTopLevel(mainWindow)!.Clipboard!);
        services.AddSingleton(_ => stringEncryptionService);
        services.AddSingleton<ILogger>(_ => logger);
        services.AddSingleton<IMessengerWrapper>(_ => new MessengerWrapper(StrongReferenceMessenger.Default));
        services.AddTransient<SimpleHtmlParser>();
        services.AddTransient<HtmlAgilityPackParser>();
        services.AddSingleton<ISettingsService, SettingsService>();
    }
}
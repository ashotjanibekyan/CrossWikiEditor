using Autofac;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.Core;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Services;
using CrossWikiEditor.Views;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace CrossWikiEditor.AutofacModules;

public sealed class ServicesModule(MainWindow mainWindow) : Module
{
    protected override void Load(ContainerBuilder builder)
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

        builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>().SingleInstance();
        builder.RegisterType<FileDialogService>().As<IFileDialogService>().SingleInstance();
        builder.RegisterType<SystemService>().As<ISystemService>().SingleInstance();
        builder.RegisterType<UserPreferencesService>().As<IUserPreferencesService>().SingleInstance();

        builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
        builder.RegisterType<PageService>().As<IPageService>().SingleInstance();
        builder.RegisterType<WikiClientCache>().As<IWikiClientCache>().SingleInstance();

        builder.RegisterType<DialogService>()
            .As<IDialogService>()
            .WithParameter(new TypedParameter(typeof(IOwner), mainWindow)).SingleInstance();
        builder.Register(c => TopLevel.GetTopLevel(mainWindow)!.StorageProvider).As<IStorageProvider>();
        builder.RegisterInstance(stringEncryptionService).As<IStringEncryptionService>();
        builder.RegisterInstance(logger).As<ILogger>();
        builder.Register(c => TopLevel.GetTopLevel(mainWindow)?.Clipboard).As<IClipboard>();
        builder.RegisterInstance(new MessengerWrapper(WeakReferenceMessenger.Default)).As<IMessengerWrapper>();
    }
}
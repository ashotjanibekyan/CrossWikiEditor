using AutoWikiEditor.Services.Implementations;
using AutoWikiEditor.Services.Interfaces;
using AutoWikiEditor.ViewModels;
using AutoWikiEditor.Views;
using Splat;

namespace AutoWikiEditor;
public static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton<IDialogService>(() => new DialogService(MainWindow.Instance, resolver));
        services.RegisterLazySingleton(() => new ViewLocator(resolver));
        services.Register<ProfilesWindowViewModel>(() => new ProfilesWindowViewModel());
    }
}

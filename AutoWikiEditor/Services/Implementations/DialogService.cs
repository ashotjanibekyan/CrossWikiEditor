using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoWikiEditor.Services.Interfaces;
using AutoWikiEditor.ViewModels;
using Avalonia.Controls;
using Splat;

namespace AutoWikiEditor.Services.Implementations;
public class DialogService : IDialogService
{
    private readonly Window _mainWindow;
    private readonly IReadonlyDependencyResolver _resolver;

    public DialogService(Window mainWindow, IReadonlyDependencyResolver resolver)
    {
        _mainWindow = mainWindow;
        _resolver = resolver;
    }

    public async Task<object> ShowDialog(Window dialog, Window? owner = null)
    {
        var result = await dialog.ShowDialog<object>(owner ?? _mainWindow);
        return result;
    }

    public Task<TResult> ShowDialog<TResult, TViewModel>(Window? owner = null) where TViewModel : ViewModelBase, IDialogViewModel<TResult>
    {
        var locator = _resolver.GetService<ViewLocator>();
        var window = locator.GetWindowFromViewModel<TViewModel>();
        var result = window.ShowDialog<TResult>(owner ?? _mainWindow);
        return result;
    }
}

using System.Threading.Tasks;
using Autofac;
using Avalonia.Controls;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.Services;

public interface IDialogService
{
    Task<TResult> ShowDialog<TResult>(ViewModelBase viewModel);
    Task Alert(string title, string content);
}

public sealed class DialogService : IDialogService
{
    private readonly IContainer _container;
    private readonly Window _mainWindow;

    public DialogService(IContainer container, Window mainWindow)
    {
        _container = container;
        _mainWindow = mainWindow;
    }
    
    public async Task<TResult> ShowDialog<TResult>(ViewModelBase viewModel)
    {
        Window dialog = _container.ResolveNamed<Window>(viewModel.GetType().Name);
        dialog.DataContext = viewModel;
        return await dialog.ShowDialog<TResult>(_mainWindow);
    }

    public async Task Alert(string title, string content)
    {
        var viewModel = new AlertViewModel(title, content);
        Window dialog = _container.ResolveNamed<Window>(nameof(AlertViewModel));
        dialog.DataContext = viewModel;
        await dialog.ShowDialog(_mainWindow);
    }
}
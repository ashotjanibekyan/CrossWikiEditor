using System.Threading.Tasks;
using Autofac;
using Avalonia.Controls;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.Services;

public interface IDialogService
{
    Task<TResult> ShowDialog<TResult>(ViewModelBase viewModel);
}

public class DialogService : IDialogService
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
        var dialog = _container.ResolveNamed<Window>(viewModel.GetType().Name);
        dialog.DataContext = viewModel;
        return await dialog.ShowDialog<TResult>(_mainWindow);
    }
}
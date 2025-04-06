using System;
using System.Threading.Tasks;
using CrossWikiEditor.Core;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CrossWikiEditor.Services;

public sealed class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOwner _mainWindow;

    public DialogService(IServiceProvider serviceProvider, IOwner mainWindow)
    {
        _serviceProvider = serviceProvider;
        _mainWindow = mainWindow;
    }

    public async Task<TResult?> ShowDialog<TResult>(ViewModelBase viewModel)
    {
        IDialog dialog = _serviceProvider.GetRequiredKeyedService<IDialog>(viewModel.GetType().Name);
        dialog.DataContext = viewModel;
        return await dialog.ShowDialog<TResult>(_mainWindow);
    }

    public async Task Alert(string title, string content)
    {
        var viewModel = new AlertViewModel(title, content);
        IDialog dialog = _serviceProvider.GetRequiredKeyedService<IDialog>(nameof(AlertViewModel));
        dialog.DataContext = viewModel;
        await dialog.ShowDialog(_mainWindow);
    }
}
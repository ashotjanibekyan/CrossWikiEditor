using CrossWikiEditor.Core;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CrossWikiEditor.Services;

public sealed class DialogService(IServiceProvider serviceProvider, IOwner mainWindow) : IDialogService
{
    public async Task<TResult?> ShowDialog<TResult>(ViewModelBase viewModel)
    {
        IDialog dialog = serviceProvider.GetRequiredKeyedService<IDialog>(viewModel.GetType().Name);
        dialog.DataContext = viewModel;
        return await dialog.ShowDialog<TResult>(mainWindow);
    }

    public async Task Alert(string title, string content)
    {
        var viewModel = new AlertViewModel(title, content);
        IDialog dialog = serviceProvider.GetRequiredKeyedService<IDialog>(nameof(AlertViewModel));
        dialog.DataContext = viewModel;
        await dialog.ShowDialog(mainWindow);
    }
}
using System.Threading.Tasks;
using Autofac;
using CrossWikiEditor.Core;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.ViewModels;

namespace CrossWikiEditor.Services;

public sealed class DialogService(IContainer container, IOwner mainWindow) : IDialogService
{
    public async Task<TResult?> ShowDialog<TResult>(ViewModelBase viewModel)
    {
        IDialog dialog = container.ResolveNamed<IDialog>(viewModel.GetType().Name);
        dialog.DataContext = viewModel;
        return await dialog.ShowDialog<TResult>(mainWindow);
    }

    public async Task Alert(string title, string content)
    {
        var viewModel = new AlertViewModel(title, content);
        IDialog dialog = container.ResolveNamed<IDialog>(nameof(AlertViewModel));
        dialog.DataContext = viewModel;
        await dialog.ShowDialog(mainWindow);
    }
}
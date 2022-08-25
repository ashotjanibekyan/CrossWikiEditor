using Avalonia.Controls;
using System.Threading.Tasks;
using AutoWikiEditor.ViewModels;

namespace AutoWikiEditor.Services.Interfaces;
public interface IDialogService
{
    Task<object> ShowDialog(Window dialog, Window? owner = null);
    Task<TResult> ShowDialog<TResult, TViewModel>(Window? owner = null) where TViewModel : ViewModelBase, IDialogViewModel<TResult>;
}

namespace CrossWikiEditor.Core.Services;

public interface IDialogService
{
    Task<TResult?> ShowDialog<TResult>(ViewModelBase viewModel);
    Task Alert(string title, string content);
}

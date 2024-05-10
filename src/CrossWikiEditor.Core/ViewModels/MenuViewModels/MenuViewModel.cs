namespace CrossWikiEditor.Core.ViewModels.MenuViewModels;

public sealed class MenuViewModel(FileMenuViewModel fileMenuViewModel) : ViewModelBase
{
    public FileMenuViewModel FileMenuViewModel { get; } = fileMenuViewModel;
}
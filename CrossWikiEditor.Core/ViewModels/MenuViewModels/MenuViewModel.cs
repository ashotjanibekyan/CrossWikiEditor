namespace CrossWikiEditor.Core.ViewModels.MenuViewModels;

public class MenuViewModel(FileMenuViewModel fileMenuViewModel) : ViewModelBase
{
    public FileMenuViewModel FileMenuViewModel { get; } = fileMenuViewModel;
}
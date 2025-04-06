namespace CrossWikiEditor.Core.ViewModels.MenuViewModels;

public sealed class MenuViewModel : ViewModelBase
{
    public MenuViewModel(FileMenuViewModel fileMenuViewModel)
    {
        FileMenuViewModel = fileMenuViewModel;
    }

    public FileMenuViewModel FileMenuViewModel { get; }
}
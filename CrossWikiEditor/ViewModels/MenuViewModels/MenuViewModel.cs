namespace CrossWikiEditor.ViewModels.MenuViewModels;

public class MenuViewModel : ViewModelBase
{
    public MenuViewModel(FileMenuViewModel fileMenuViewModel)
    {
        FileMenuViewModel = fileMenuViewModel;
    }

    public FileMenuViewModel FileMenuViewModel { get; }
}
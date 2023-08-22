using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class MenuViewModel : ViewModelBase
{
    private readonly Window _mainWindow;

    public MenuViewModel(Window mainWindow)
    {
        _mainWindow = mainWindow;
        ExitCommand = ReactiveCommand.Create(this.Exit);
    }

    public void Exit()
    {
        _mainWindow.Close();
    }
    
    public ReactiveCommand<Unit, Unit> ExitCommand { get; set; }
}
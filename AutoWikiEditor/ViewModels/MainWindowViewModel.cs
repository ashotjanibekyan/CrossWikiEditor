using System.Collections.Generic;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace AutoWikiEditor.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        this.ExitCommand = ReactiveCommand.Create<Window>(this.Exit);
        this.MakeListViewModel = new MakeListViewModel();
    }

    public ReactiveCommand<Window, Unit> ExitCommand { get; }

    public MakeListViewModel MakeListViewModel { get; set; }
    private void Exit(Window window)
    {
        window.Close();
    }
}
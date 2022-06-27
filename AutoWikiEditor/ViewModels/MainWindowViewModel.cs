using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace AutoWikiEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            this.ExitCommand = ReactiveCommand.Create<Window>(this.Exit);
        }

        public ReactiveCommand<Window, Unit> ExitCommand { get; }

        private void Exit(Window window)
        {
            window.Close();
        }
    }
}
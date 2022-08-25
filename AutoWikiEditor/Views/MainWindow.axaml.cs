using AutoWikiEditor.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoWikiEditor.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }
    }
}
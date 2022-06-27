using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoWikiEditor.Views
{
    public partial class StatusBarView : UserControl
    {
        public StatusBarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

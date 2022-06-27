using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoWikiEditor.Views.ControlViews
{
    public partial class StartView : UserControl
    {
        public StartView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

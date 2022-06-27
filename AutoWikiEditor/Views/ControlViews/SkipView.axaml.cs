using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoWikiEditor.Views.ControlViews
{
    public partial class SkipView : UserControl
    {
        public SkipView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

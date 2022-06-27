using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoWikiEditor.Views.ControlViews
{
    public partial class MoreView : UserControl
    {
        public MoreView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

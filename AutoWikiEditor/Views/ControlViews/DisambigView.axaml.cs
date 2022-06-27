using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoWikiEditor.Views.ControlViews
{
    public partial class DisambigView : UserControl
    {
        public DisambigView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

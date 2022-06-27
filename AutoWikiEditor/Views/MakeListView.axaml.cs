using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoWikiEditor.Views
{
    public partial class MakeListView : UserControl
    {
        public MakeListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

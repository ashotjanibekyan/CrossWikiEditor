using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrossWikiEditor.Views.ControlViews;

public sealed partial class StartView : UserControl
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
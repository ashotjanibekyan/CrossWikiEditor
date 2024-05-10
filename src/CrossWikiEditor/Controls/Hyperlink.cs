using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;

namespace CrossWikiEditor.Controls;

public sealed class Hyperlink : InlineUIContainer
{
    private readonly Underline _underline;
    private readonly TextBlock _textBlock;
    private readonly Button _button;
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<Hyperlink, string>("Text");
    public static readonly StyledProperty<string> HrefProperty = AvaloniaProperty.Register<Hyperlink, string>("Href");

    public Span Content => _underline;

    public string Text
    {
        get => GetValue(TextProperty);
        set
        {
            _textBlock.Text = value;
            SetValue(TextProperty, value);
        }
    }

    public string Href
    {
        get => GetValue(HrefProperty);
        set
        {
            ToolTip.SetTip(_button, value);
            SetValue(HrefProperty, value);
        }
    }

    public Hyperlink()
    {
        _underline = new Underline();

        _textBlock = new TextBlock
        {
            Inlines = new InlineCollection
            {
                _underline
            }
        };

        _button = new Button
        {
            Background = Brushes.Transparent,
            Margin = new Thickness(),
            Padding = new Thickness(),
            Cursor = new Cursor(StandardCursorType.Hand),
            Content = _textBlock,
            Command = new RelayCommand(() => OpenUrl(Href))
        };

        Child = _button;
    }

    private static void OpenUrl(string url)
    {
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}
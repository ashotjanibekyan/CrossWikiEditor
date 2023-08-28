using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace CrossWikiEditor.Models;

public class DiffRowNode
{
    public List<string> HtmlClasses { get; set; } = new();
    public string Marker { get; set; } = string.Empty;
    public List<ContentNode> ContentNodes { get; set; } = new();
    public int ColSpan { get; set; } = 1;

    public TextBlock GetTextBlock()
    {
        var text = new TextBlock
        {
            TextWrapping = TextWrapping.WrapWithOverflow,
            Text = Marker
        };
        foreach (var node in ContentNodes)
        {

            var run = new Run(node.Value );
            if (node.Name == "del")
            {
                run.Foreground = Brushes.Brown;
            }

            if (node.Name == "ins")
            {
                run.Foreground = Brushes.Blue;
            }

            if (node.Classes.Contains("diff-addedline"))
            {
                run.Background = Brushes.Blue;
                run.Text = " h ";
                text.Width = 30;
            }
            
            if (node.Classes.Contains("diff-deletedline"))
            {
                text.Background = Brushes.Red;
                text.Text = " h ";
                text.Width = 30;
            }
            text.Inlines.Add(run);
        }

        return text;
    }
}

public class ContentNode
{
    public string Value { get; set; }
    public List<string> Classes { get; set; }
    public string Name { get; set; }
}
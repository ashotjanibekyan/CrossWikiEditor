using Avalonia.Data.Converters;
using CrossWikiEditor.Models;

namespace CrossWikiEditor.Utils;

public static class Converters
{
    public static readonly IValueConverter IsNullOrWhiteSpace =
        new FuncValueConverter<string?, bool>(string.IsNullOrWhiteSpace);
    
    public static readonly IValueConverter DiffRowNodeToText =
        new FuncValueConverter<DiffRowNode, string>(node =>
        {
            if (!string.IsNullOrWhiteSpace(node?.Content))
            {
                return node.Content;
            }

            return node.Marker;
        });

    public static readonly IValueConverter IsNotNullOrWhiteSpace =
        new FuncValueConverter<string?, bool>(x => !string.IsNullOrWhiteSpace(x));

    public static readonly IValueConverter SetOperationsConverter =
        new FuncValueConverter<SetOperations, string?>(x => x switch
        {
            SetOperations.Intersection => "Intersection",
            SetOperations.SymmetricDifference => "Symmetric difference",
            _ => x.ToString()
        });
}
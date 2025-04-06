using System.Linq;
using Avalonia.Data.Converters;
using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Converters;

public static class Converters
{
    public static readonly IValueConverter IsNullOrWhiteSpace =
        new FuncValueConverter<string?, bool>(string.IsNullOrWhiteSpace);

    public static readonly IValueConverter IsNotNullOrWhiteSpace =
        new FuncValueConverter<string?, bool>(x => !string.IsNullOrWhiteSpace(x));

    public static readonly IValueConverter SetOperationsConverter =
        new FuncValueConverter<SetOperations, string?>(x => x switch
        {
            SetOperations.Intersection => "Intersection",
            SetOperations.SymmetricDifference => "Symmetric difference",
            _ => x.ToString()
        });

    public static readonly IMultiValueConverter FirstEqualsToAnyConverter =
        new FuncMultiValueConverter<object?, bool>(items =>
        {
            var itemsList = items.ToList();
            if (itemsList.Count <= 1)
            {
                return true;
            }

            object? first = itemsList[0];
            if (first == null)
            {
                return false;
            }

            for (int i = 1; i < itemsList.Count; i++)
            {
                if (first.Equals(itemsList[i]))
                {
                    return true;
                }
            }

            return false;
        });
}
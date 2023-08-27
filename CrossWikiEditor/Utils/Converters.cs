using Avalonia.Data.Converters;

namespace CrossWikiEditor.Utils;

public static class Converters
{
    public static readonly IValueConverter IsNullOrWhiteSpace =
        new FuncValueConverter<string?, bool>(string.IsNullOrWhiteSpace);
    
    public static readonly IValueConverter IsNotNullOrWhiteSpace =
        new FuncValueConverter<string?, bool>(x => !string.IsNullOrWhiteSpace(x));
}
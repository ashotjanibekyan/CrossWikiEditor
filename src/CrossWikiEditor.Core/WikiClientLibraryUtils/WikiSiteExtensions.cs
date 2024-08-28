namespace CrossWikiEditor.Core.WikiClientLibraryUtils;

public static class WikiSiteExtensions
{
    public static string? ToString(this PropertyFilterOption value,
        string? withValue, string? withoutValue, string? allValue = "all")
    {
        return value switch
        {
            PropertyFilterOption.Disable => allValue,
            PropertyFilterOption.WithProperty => withValue,
            PropertyFilterOption.WithoutProperty => withoutValue,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
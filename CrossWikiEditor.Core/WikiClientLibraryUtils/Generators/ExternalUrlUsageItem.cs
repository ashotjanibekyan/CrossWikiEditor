namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class ExternalUrlUsageItem
{
    public required int PageId { get; set; }
    public required int NamespaceId { get; set; }
    public required string Title { get; set; }
    public required string Url { get; set; }
}
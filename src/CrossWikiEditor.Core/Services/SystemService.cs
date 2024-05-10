namespace CrossWikiEditor.Core.Services;

public interface ISystemService
{
    Result OpenLinkInBrowser(string url);
    Task<string?> GetClipboardTextAsync();
    Task SetClipboardTextAsync(string? text);
    Task WriteAllLinesAsync(string path, IEnumerable<string> contents);
    Task<string> ReadAllTextAsync(string path, Encoding encoding);
}
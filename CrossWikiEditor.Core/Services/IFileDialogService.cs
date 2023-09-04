namespace CrossWikiEditor.Core.Services;

public interface IFileDialogService
{
    Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<string>? patterns = null);

    Task<(Func<Task<Stream>> openReadStream, Func<Task<Stream>> openWriteStream)> SaveFilePickerAsync(string title, string? defaultExtension = null, string? suggestedFileName = null);
}
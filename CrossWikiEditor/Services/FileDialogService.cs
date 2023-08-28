using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Platform.Storage;

namespace CrossWikiEditor.Services;

public interface IFileDialogService
{
    Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<FilePickerFileType> filters);

    Task<IStorageFile?> SaveFilePickerAsync(string title, string? defaultExtension = null, string? suggestedFileName = null);
}

public sealed class FileDialogService(IStorageProvider storageProvider) : IFileDialogService
{
    public async Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<FilePickerFileType> filters)
    {
        IReadOnlyList<IStorageFile> result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = filters
        });
        return result.Select(f => HttpUtility.UrlDecode(f.Path.AbsolutePath)).ToArray();
    }

    public async Task<IStorageFile?> SaveFilePickerAsync(string title, string? defaultExtension = null, string? suggestedFileName = null)
    {
        return await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = title,
            ShowOverwritePrompt = true,
            DefaultExtension = defaultExtension,
            SuggestedFileName = suggestedFileName
        });
    }
}
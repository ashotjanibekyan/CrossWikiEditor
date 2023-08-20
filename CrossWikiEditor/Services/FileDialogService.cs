using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace CrossWikiEditor.Services;

public interface IFileDialogService
{
    Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<FilePickerFileType> filters);
}

public sealed class FileDialogService : IFileDialogService
{
    private readonly IStorageProvider _storageProvider;

    public FileDialogService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }


    public async  Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<FilePickerFileType> filters)
    {
        return (await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = filters,
        })).Select(f => f.Path.AbsolutePath).ToArray();
    }
}

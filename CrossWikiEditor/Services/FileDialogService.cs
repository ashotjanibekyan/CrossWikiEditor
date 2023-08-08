using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace CrossWikiEditor.Services;

public interface IFileDialogService
{
    Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<FilePickerFileType> filters);
}

public class FileDialogService : IFileDialogService
{
    readonly Window _parent;

    public FileDialogService(Window parent) => _parent = parent;
    

    public async  Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<FilePickerFileType> filters)
    {
        return (await TopLevel.GetTopLevel(_parent).StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = filters,
        })).Select(f => f.Path.AbsolutePath).ToArray();
    }
}

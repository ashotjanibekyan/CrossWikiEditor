using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Platform.Storage;
using CrossWikiEditor.Core.Services;

namespace CrossWikiEditor.Services;

public sealed class FileDialogService(IStorageProvider storageProvider) : IFileDialogService
{
    public async Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<string>? patterns = null)
    {
        IReadOnlyList<IStorageFile> result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType(null)
                {
                    Patterns = patterns
                }
            }
        });
        return result.Select(f => HttpUtility.UrlDecode(f.Path.AbsolutePath)).ToArray();
    }

    public async Task<(Func<Task<Stream>> openReadStream, Func<Task<Stream>> openWriteStream)> SaveFilePickerAsync(string title, string? defaultExtension = null, string? suggestedFileName = null)
    {
        IStorageFile? storageFile = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = title,
            ShowOverwritePrompt = true,
            DefaultExtension = defaultExtension,
            SuggestedFileName = suggestedFileName
        });
        if (storageFile is null)
        {
            throw new InvalidOperationException();
        }

        return (storageFile.OpenReadAsync, storageFile.OpenWriteAsync);
    }
}
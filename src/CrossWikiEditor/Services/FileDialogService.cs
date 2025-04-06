using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Platform.Storage;
using CrossWikiEditor.Core.Services;

namespace CrossWikiEditor.Services;

public sealed class FileDialogService : IFileDialogService
{
    private readonly IStorageProvider _storageProvider;

    public FileDialogService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<string[]?> OpenFilePickerAsync(
        string title,
        bool allowMultiple,
        List<string>? patterns = null)
    {
        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple
        };
        if (patterns is not null)
        {
            options.FileTypeFilter = [new FilePickerFileType("Select file...") {Patterns = patterns}];
        }

        IReadOnlyList<IStorageFile> result = await _storageProvider.OpenFilePickerAsync(options);
        return result.Select(f => HttpUtility.UrlDecode(f.Path.AbsolutePath)).ToArray();
    }

    public async Task<(Func<Task<Stream>>? openReadStream, Func<Task<Stream>>? openWriteStream)> SaveFilePickerAsync(string title,
        string? defaultExtension = null, string? suggestedFileName = null)
    {
        IStorageFile? storageFile = await _storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            ShowOverwritePrompt = true,
            DefaultExtension = defaultExtension,
            SuggestedFileName = suggestedFileName
        });
        return storageFile is null ? (null, null) : (storageFile.OpenReadAsync, storageFile.OpenWriteAsync);
    }
}
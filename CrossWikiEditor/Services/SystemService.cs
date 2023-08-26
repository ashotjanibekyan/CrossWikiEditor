using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input.Platform;

namespace CrossWikiEditor.Services;

public interface ISystemService
{
    Result OpenLinkInBrowser(string url);
    Task<string?> GetClipboardTextAsync();
    Task SetClipboardTextAsync(string? text);
    Task WriteAllLinesAsync(string path, IEnumerable<string> contents);
}

public class SystemService : ISystemService
{
    private readonly IClipboard _clipboard;

    public SystemService(IClipboard clipboard)
    {
        _clipboard = clipboard;
    }
    public Result OpenLinkInBrowser(string url)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    public async Task<string?> GetClipboardTextAsync()
    { 
        return await _clipboard.GetTextAsync();
    }

    public async Task SetClipboardTextAsync(string? text)
    {
        await _clipboard.SetTextAsync(text);
    }

    public async Task WriteAllLinesAsync(string path, IEnumerable<string> contents)
    {
        await File.WriteAllLinesAsync(path, contents, encoding: Encoding.UTF8);
    }
}
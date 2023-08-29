using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Serilog;

namespace CrossWikiEditor.Services;

public interface ISystemService
{
    Result OpenLinkInBrowser(string url);
    Task<string?> GetClipboardTextAsync();
    Task SetClipboardTextAsync(string? text);
    Task WriteAllLinesAsync(string path, IEnumerable<string> contents);
    Task<string> ReadAllTextAsync(string path, Encoding encoding);
}

public class SystemService(IClipboard clipboard, ILogger logger) : ISystemService
{
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
            logger.Fatal(e, "Failed to open link in browser");
            return Result.Failure(e.Message);
        }
    }

    public async Task<string?> GetClipboardTextAsync()
    {
        return await clipboard.GetTextAsync();
    }

    public async Task SetClipboardTextAsync(string? text)
    {
        await clipboard.SetTextAsync(text);
    }

    public async Task WriteAllLinesAsync(string path, IEnumerable<string> contents)
    {
        await File.WriteAllLinesAsync(path, contents, Encoding.UTF8);
    }

    public async Task<string> ReadAllTextAsync(string path, Encoding encoding)
    {
        return await File.ReadAllTextAsync(path, encoding);
    }
}
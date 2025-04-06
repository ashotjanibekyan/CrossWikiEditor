using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Utils;
using Serilog;

namespace CrossWikiEditor.Services;

public sealed class SystemService : ISystemService
{
    private readonly IClipboard _clipboard;
    private readonly ILogger _logger;

    public SystemService(IClipboard clipboard, ILogger logger)
    {
        _clipboard = clipboard;
        _logger = logger;
    }

    public Result<Unit> OpenLinkInBrowser(string url)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
            return Unit.Default;
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Failed to open link in browser");
            return e;
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
        await File.WriteAllLinesAsync(path, contents, Encoding.UTF8);
    }

    public async Task<string> ReadAllTextAsync(string path, Encoding encoding)
    {
        return await File.ReadAllTextAsync(path, encoding);
    }
}
using System.Diagnostics;
using System.Text;

using Avalonia.Input.Platform;

using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Utils;

using Serilog;

namespace CrossWikiEditor.Services;

public sealed class SystemService(IClipboard clipboard, ILogger logger) : ISystemService
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
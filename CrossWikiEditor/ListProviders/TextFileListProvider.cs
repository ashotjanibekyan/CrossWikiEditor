using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.ListProviders;

public class TextFileListProvider : IListProvider
{
    private readonly IFileDialogService _fileDialogService;
    private readonly ISystemService _systemService;
    private readonly IWikiClientCache _wikiClientCache;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly List<string> _textFiles = new();

    public TextFileListProvider(IFileDialogService fileDialogService,
        ISystemService systemService,
        IWikiClientCache wikiClientCache,
        IUserPreferencesService userPreferencesService)
    {
        _fileDialogService = fileDialogService;
        _systemService = systemService;
        _wikiClientCache = wikiClientCache;
        _userPreferencesService = userPreferencesService;
    }
    public string Title => "Text file";
    public string ParamTitle => string.Empty;
    public string Param { get; set; } = string.Empty;
    public bool CanMake => _textFiles.Any();
    public bool NeedsAdditionalParams { get; set; } = true;
    public async Task<Result<List<WikiPageModel>>> MakeList()
    {
        WikiSite site = await _wikiClientCache.GetWikiSite(_userPreferencesService.GetCurrentPref().UrlApi());
        var titles = new List<string>();
        var result = new List<WikiPageModel>();
        foreach (string textFile in _textFiles)
        {
            string pageText = await _systemService.ReadAllTextAsync(textFile, Encoding.Default);
            if (Tools.WikiLinkRegex().IsMatch(pageText))
            {
                titles.AddRange(Tools.WikiLinkRegex()
                    .Matches(pageText)
                    .Select(m => m.Groups[1].Value)
                    .Where(title => !Tools.FromFileRegex().IsMatch(title) && !title.StartsWith("#"))
                    .Select(Tools.RemoveSyntax));
            }
            else
            {
                titles.AddRange(pageText.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => s.Trim().Length != 0)
                    .Select(Tools.RemoveSyntax));
            }
        }
        foreach (string title in titles)
        {
            try
            {
                var page = new WikiPage(site, title);
                result.Add(new WikiPageModel(page));
            }
            catch (ArgumentException)
            {
            }
        }
        _textFiles.Clear();
        return Result<List<WikiPageModel>>.Success(result);
    }
    
    public async Task GetAdditionalParams()
    {
        string[]? result = await _fileDialogService.OpenFilePickerAsync("Select text files to extract pages", true, new List<FilePickerFileType>());
        if (result is not null)
        {
            _textFiles.AddRange(result);
        }
    }
}
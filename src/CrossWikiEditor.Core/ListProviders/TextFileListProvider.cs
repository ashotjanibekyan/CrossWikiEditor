using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class TextFileListProvider : UnlimitedListProviderBase, INeedAdditionalParamsListProvider
{
    private readonly List<string> _textFiles = [];
    private readonly IFileDialogService _fileDialogService;
    private readonly ISystemService _systemService;
    private readonly ISettingsService _settingsService;
    private readonly IWikiClientCache _wikiClientCache;

    public TextFileListProvider(IFileDialogService fileDialogService,
        ISystemService systemService,
        ISettingsService settingsService,
        IWikiClientCache wikiClientCache)
    {
        _fileDialogService = fileDialogService;
        _systemService = systemService;
        _settingsService = settingsService;
        _wikiClientCache = wikiClientCache;
    }

    public override string Title => "Text file";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _textFiles.Count != 0;

    public async Task GetAdditionalParams()
    {
        string[]? result = await _fileDialogService.OpenFilePickerAsync("Select text files to extract pages", true);
        if (result is not null)
        {
            _textFiles.AddRange(result);
        }
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        var titles = new List<string>();
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

        List<WikiPageModel> result = titles.ConvertAll(title => new WikiPageModel(title, _settingsService.CurrentApiUrl, _wikiClientCache));

        _textFiles.Clear();
        return result;
    }
}
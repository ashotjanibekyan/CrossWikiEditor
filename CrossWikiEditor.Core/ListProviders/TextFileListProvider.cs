﻿namespace CrossWikiEditor.Core.ListProviders;

public sealed class TextFileListProvider(IFileDialogService fileDialogService,
        ISystemService systemService,
        IUserPreferencesService userPreferencesService,
        IWikiClientCache wikiClientCache)
    : UnlimitedListProviderBase, INeedAdditionalParamsListProvider
{
    private readonly List<string> _textFiles = new();

    public override string Title => "Text file";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _textFiles.Count != 0;

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        var titles = new List<string>();
        foreach (string textFile in _textFiles)
        {
            string pageText = await systemService.ReadAllTextAsync(textFile, Encoding.Default);
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

        var result = titles.Select(title => new WikiPageModel(title, userPreferencesService.CurrentApiUrl, wikiClientCache)).ToList();

        _textFiles.Clear();
        return Result<List<WikiPageModel>>.Success(result);
    }

    public async Task GetAdditionalParams()
    {
        string[]? result = await fileDialogService.OpenFilePickerAsync("Select text files to extract pages", true);
        if (result is not null)
        {
            _textFiles.AddRange(result);
        }
    }
}
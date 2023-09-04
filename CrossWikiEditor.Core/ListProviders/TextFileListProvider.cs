using System.Text;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.ListProviders;

public class TextFileListProvider(
        IFileDialogService fileDialogService,
        ISystemService systemService,
        IWikiClientCache wikiClientCache,
        IUserPreferencesService userPreferencesService)
    : UnlimitedListProviderBase, INeedAdditionalParamsListProvider
{
    private readonly List<string> _textFiles = new();

    public override string Title => "Text file";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _textFiles.Count != 0;

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        WikiSite site = await wikiClientCache.GetWikiSite(userPreferencesService.GetCurrentPref().UrlApi());
        var titles = new List<string>();
        var result = new List<WikiPageModel>();
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
        string[]? result = await fileDialogService.OpenFilePickerAsync("Select text files to extract pages", true);
        if (result is not null)
        {
            _textFiles.AddRange(result);
        }
    }
}
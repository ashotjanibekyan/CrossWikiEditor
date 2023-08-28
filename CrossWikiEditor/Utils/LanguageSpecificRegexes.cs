using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.WikiClientLibraryUtils;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Utils;

public class LanguageSpecificRegexes : IAsyncInitialization
{
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IWikiClientCache _wikiClientCache;
    private WikiSite _site;
    private MagicWordCollection _magicWordCollection;

    public LanguageSpecificRegexes(
        IUserPreferencesService userPreferencesService,
        IWikiClientCache wikiClientCache,
        IMessenger messenger)
    {
        _userPreferencesService = userPreferencesService;
        _wikiClientCache = wikiClientCache;
        InitAsync = InitializeAsync();
        messenger.Register<LanguageCodeChangedMessage>(this, (_, _) => InitAsync = InitializeAsync());
        messenger.Register<ProjectChangedMessage>(this, (_, _) => InitAsync = InitializeAsync());
    }

    private async Task InitializeAsync()
    {
        string apiRoot = _userPreferencesService.GetCurrentPref().UrlApi();
        _site = await _wikiClientCache.GetWikiSite(apiRoot);
        _magicWordCollection = await _site.GetMagicWords();
        MakeRegexes();
    }

    private void MakeRegexes()
    {
        string? url = _userPreferencesService.GetCurrentPref().UrlBase();
        string? urlLong = _userPreferencesService.GetCurrentPref().UrlBaseLong();

        int pos = Tools.FirstDifference(url, urlLong);
        string s = Regex.Escape(urlLong[..pos]).Replace(@"https://", @"https?://");
        s += "(?:" + Regex.Escape(urlLong[pos..]) + @"index\.php(?:\?title=|/)|"
             + Regex.Escape(url[pos..]) + "/wiki/" + ")";
        ExtractTitle = new Regex("^" + s + "([^?&]*)$");
    }

    public Regex ExtractTitle { get; private set; }
    public Task InitAsync { get; private set; }
}
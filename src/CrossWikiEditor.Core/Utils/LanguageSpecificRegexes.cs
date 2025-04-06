using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.Utils;

public sealed class LanguageSpecificRegexes : IAsyncInitialization
{
    private readonly ISettingsService _settingsService;
    private readonly IWikiClientCache _wikiClientCache;
    private WikiSite? _site;

    public LanguageSpecificRegexes(
        ISettingsService settingsService,
        IWikiClientCache wikiClientCache,
        IMessengerWrapper messenger)
    {
        _settingsService = settingsService;
        _wikiClientCache = wikiClientCache;
        InitAsync = InitializeAsync();
        messenger.Register<LanguageCodeChangedMessage>(this, (_, _) => InitAsync = InitializeAsync());
        messenger.Register<ProjectChangedMessage>(this, (_, _) => InitAsync = InitializeAsync());
    }

    public Regex? ExtractTitle { get; private set; }
    public Task InitAsync { get; private set; }

    private async Task InitializeAsync()
    {
        string apiRoot = _settingsService.CurrentApiUrl;
        _site = await _wikiClientCache.GetWikiSite(apiRoot);
        MakeRegexes();
    }

    private void MakeRegexes()
    {
        string? url = _settingsService.GetCurrentSettings().GetBaseUrl();
        string? urlLong = _settingsService.GetCurrentSettings().GetLongBaseUrl();

        int pos = Tools.FirstDifference(url, urlLong);
        string s = Regex.Escape(urlLong[..pos]).Replace("https://", "https?://");
        s += "(?:" + Regex.Escape(urlLong[pos..]) + @"index\.php(?:\?title=|/)|"
             + Regex.Escape(url[pos..]) + "/wiki/)";
        ExtractTitle = new Regex("^" + s + "([^?&]*)$");
    }
}
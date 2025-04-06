using System;
using System.Threading.Tasks;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.Utils.Extensions;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.Models;

public sealed class WikiPageModel : IEquatable<WikiPageModel>, IComparable<WikiPageModel>, IAsyncInitialization
{
    private readonly string _apiRoot;
    private readonly IWikiClientCache? _wikiClientCache;

    private Task? _initAsync;
    private WikiPage? _wikiPage;

    public WikiPageModel(WikiPage wikiPage)
    {
        _initAsync = Task.CompletedTask;
        _wikiPage = wikiPage;
        _apiRoot = wikiPage.Site.ApiEndpoint;
        Title = wikiPage.Title ?? string.Empty;
        NamespaceId = wikiPage.NamespaceId;
    }

    public WikiPageModel(string title, string apiRoot, IWikiClientCache wikiClientCache)
    {
        _apiRoot = apiRoot;
        _wikiClientCache = wikiClientCache;
        Title = title;
    }

    public string Title { get; }

    public int NamespaceId { get; set; }

    public Task InitAsync => _initAsync ??= InitializeAsync(Title, _apiRoot, _wikiClientCache!);

    public int CompareTo(WikiPageModel? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        return ReferenceEquals(null, other) ? 1 : string.CompareOrdinal(Title, other.Title);
    }

    public bool Equals(WikiPageModel? other)
    {
        return this == other;
    }

    private async Task InitializeAsync(string title, string apiRoot, IWikiClientCache wikiClientCache)
    {
        WikiSite site = await wikiClientCache.GetWikiSite(apiRoot);
        await site.Initialization;
        _wikiPage = new WikiPage(site, title);
        await _wikiPage.RefreshAsync();
        NamespaceId = _wikiPage.NamespaceId;
    }

    public async Task<string> GetContent()
    {
        await InitAsync;
        return _wikiPage?.Content ?? string.Empty;
    }

    public async Task<bool> EditAsync(string content, string summary = "", bool isBot = true, bool isMinor = false)
    {
        await InitAsync;
        if (_wikiPage is null)
        {
            return false;
        }

        return await _wikiPage.EditAsync(new WikiPageEditOptions
        {
            Content = content,
            Summary = summary,
            Bot = isBot,
            Minor = isMinor
        });
    }

    public async Task<bool> Exists()
    {
        await InitAsync;
        return _wikiPage!.Exists;
    }

    public async Task RefreshAsync(PageQueryOptions fetchContent)
    {
        await _wikiPage!.RefreshAsync(fetchContent);
    }

    public WikiPageModel ToTalkPage()
    {
        return new WikiPageModel(_wikiPage!.ToTalkPage());
    }

    public WikiPageModel ToSubjectPage()
    {
        return new WikiPageModel(_wikiPage!.ToSubjectPage());
    }

    public void Deconstruct(out string title, out int namespaceId)
    {
        title = Title;
        namespaceId = NamespaceId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is WikiPageModel wikiPageModel)
        {
            return this == wikiPageModel;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Title);
    }

    public override string ToString()
    {
        return $"WikiPageModel {{ Title = {Title}, Namespace: {NamespaceId}, WikiPage: {_wikiPage} }} ";
    }

    public static bool operator ==(WikiPageModel? left, WikiPageModel? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Title == right.Title;
    }

    public static bool operator !=(WikiPageModel? left, WikiPageModel? right)
    {
        return !(left == right);
    }
}
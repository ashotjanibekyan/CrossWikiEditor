using System.Linq;
using CrossWikiEditor.Models;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Utils;

public static class WikiPageExtensions
{
    public static WikiPage ToTalkPage(this WikiPage wikiPage)
    {
        if (wikiPage.NamespaceId < 0 || wikiPage.NamespaceId.IsOdd())
        {
            return wikiPage;
        }

        NamespaceInfo? ns = wikiPage.Site.Namespaces[wikiPage.NamespaceId + 1];
        string nslessTitle = wikiPage.Title.Contains(':') ? wikiPage.Title.Split(':')[1] : wikiPage.Title;
        return new WikiPage(wikiPage.Site, $"{ns.CanonicalName}:{nslessTitle}");
    }

    public static WikiPage ToSubjectPage(this WikiPage wikiPage)
    {
        if (wikiPage.NamespaceId < 0 || wikiPage.NamespaceId.IsEven())
        {
            return wikiPage;
        }

        NamespaceInfo? ns = wikiPage.Site.Namespaces[wikiPage.NamespaceId - 1];
        return new WikiPage(wikiPage.Site, $"{ns.CanonicalName}:{wikiPage.TitleWithoutNamespace()}");
    }

    public static string TitleWithoutNamespace(this WikiPage wikiPage)
    {
        return wikiPage.Title.Contains(':') ? wikiPage.Title.Split(':')[1] : wikiPage.Title;
    }

    public static string TitleWithoutNamespace(this WikiPageModel wikiPage)
    {
        return wikiPage.Title.Contains(':') ? wikiPage.Title.Split(':')[1] : wikiPage.Title;
    }

    public static bool ShouldKeepPer(this WikiPageModel page, FilterOptions options)
    {
        bool shouldAdd = !(options.NamespacesToKeep.Any() && !options.NamespacesToKeep.Contains(page.NamespaceId));
        if (options.RemoveTitlesContaining != string.Empty && page.Title.Contains(options.RemoveTitlesContaining, options.UseRegex))
        {
            shouldAdd = false;
        }

        if (options.KeepTitlesContaining != string.Empty && !page.Title.Contains(options.KeepTitlesContaining, options.UseRegex))
        {
            shouldAdd = false;
        }

        return shouldAdd;
    }
}
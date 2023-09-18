namespace CrossWikiEditor.Core.Utils;

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

    private static string TitleWithoutNamespace(this WikiPage wikiPage)
    {
        return wikiPage.Title.Contains(':') ? wikiPage.Title.Split(':')[1] : wikiPage.Title;
    }
}
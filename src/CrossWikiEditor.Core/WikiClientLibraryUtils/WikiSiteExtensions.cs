using Newtonsoft.Json.Linq;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils;

public static class WikiSiteExtensions
{
    public static async Task<MagicWordCollection> GetMagicWords(this WikiSite wikiSite)
    {
        JToken? result = await wikiSite.InvokeMediaWikiApiAsync(new MediaWikiFormRequestMessage(new
        {
            action = "query",
            meta = "siteinfo",
            siprop = "magicwords"
        }), true, CancellationToken.None);
        return new MagicWordCollection((JArray) result["query"]["magicwords"]);
    }

    public static string? ToString(this PropertyFilterOption value,
        string? withValue, string? withoutValue, string? allValue = "all")
    {
        return value switch
        {
            PropertyFilterOption.Disable => allValue,
            PropertyFilterOption.WithProperty => withValue,
            PropertyFilterOption.WithoutProperty => withoutValue,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
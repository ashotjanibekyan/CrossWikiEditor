using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.WikiClientLibraryUtils;

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
        return new MagicWordCollection((JArray)result["query"]["magicwords"]);
    }
} 
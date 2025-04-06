using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;
using WikiClientLibrary.Generators.Primitive;
using WikiClientLibrary.Infrastructures;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class ExternalUrlUsageGenerator : WikiList<ExternalUrlUsageItem>
{
    public ExternalUrlUsageGenerator(WikiSite site) : base(site)
    {
    }

    public override string ListName => "exturlusage";
    public IEnumerable<int>? NamespaceIds { get; set; }
    public string? Url { get; set; }
    public string? Protocol { get; set; }

    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        return new Dictionary<string, object?>
        {
            {"euprop", "ids|title|url"},
            {"euprotocol", Protocol},
            {"euquery", HttpUtility.UrlEncode(Url)},
            {"eunamespace", NamespaceIds == null ? null : MediaWikiHelper.JoinValues(NamespaceIds)},
            {"eulimit", PaginationSize}
        };
    }

    protected override ExternalUrlUsageItem ItemFromJson(JToken json)
    {
        var jsonObj = (JObject) json;
        JToken? pageId = jsonObj["pageid"];
        JToken? ns = jsonObj["ns"];
        JToken? title = jsonObj["title"];
        JToken? url = jsonObj["url"];
        if (pageId is null || ns is null || title is null || url is null)
        {
            throw new Exception("External url usage generation error");
        }

        return new ExternalUrlUsageItem
        {
            PageId = pageId.Value<int>(),
            NamespaceId = ns.Value<int>(),
            Title = title.Value<string>() ?? string.Empty,
            Url = url.Value<string>() ?? string.Empty
        };
    }
}
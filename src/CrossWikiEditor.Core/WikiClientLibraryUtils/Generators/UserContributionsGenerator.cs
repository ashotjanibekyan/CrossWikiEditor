using System;
using System.Collections.Generic;
using System.Linq;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.Utils.Extensions;
using Newtonsoft.Json.Linq;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Generators.Primitive;
using WikiClientLibrary.Infrastructures;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Pages.Queries;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class UserContributionsGenerator : WikiList<UserContributionResultItem>
{
    private readonly List<int>? _userIds;
    private readonly List<string>? _usernames;
    private readonly string? _userPrefix;

    /// <summary>
    ///     Create a wikilist of user contributions by usernames.
    /// </summary>
    public UserContributionsGenerator(WikiSite site, List<string> usernames) : base(site)
    {
        _usernames = usernames;
        _userIds = null;
        _userPrefix = null;
    }

    /// <summary>
    ///     Create a wikilist of user contributions by user ids.
    /// </summary>
    public UserContributionsGenerator(WikiSite site, List<int> userIds) : base(site)
    {
        _usernames = null;
        _userIds = userIds;
        _userPrefix = null;
    }

    /// <summary>
    ///     Create a wikilist of user contributions by user prefixes.
    /// </summary>
    public UserContributionsGenerator(WikiSite site, string userPrefix) : base(site)
    {
        _usernames = null;
        _userIds = null;
        _userPrefix = userPrefix;
    }

    public override string ListName => "usercontribs";

    /// <summary>
    ///     The start timestamp to return from, i.e. revisions before this timestamp.
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    ///     The end timestamp to return to, i.e. revisions after this timestamp.
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    ///     Adds the page ID and revision ID.
    /// </summary>
    public bool IncludeIds { get; set; }

    /// <summary>
    ///     Adds the title and namespace ID of the page.
    /// </summary>
    public bool IncludeTitle { get; set; }

    /// <summary>
    ///     Adds the timestamp of the edit.
    /// </summary>
    public bool IncludeTimestamp { get; set; }

    /// <summary>
    ///     Adds the comment of the edit. If the comment has been revision deleted, a commenthidden property will be returned.
    /// </summary>
    public bool IncludeComment { get; set; }

    /// <summary>
    ///     Adds the parsed comment of the edit. If the comment has been revision deleted, a commenthidden property will be
    ///     returned.
    /// </summary>
    public bool IncludeParsedComment { get; set; }

    /// <summary>
    ///     Adds the new size of the edit.
    /// </summary>
    public bool IncludeSize { get; set; }

    /// <summary>
    ///     Adds the size delta of the edit against its parent.
    /// </summary>
    public bool IncludeSizeDiff { get; set; }

    /// <summary>
    ///     Adds flags of the edit.
    /// </summary>
    public bool IncludeFlags { get; set; }

    /// <summary>
    ///     Tags patrolled edits.
    /// </summary>
    public bool IncludePatrolled { get; set; }

    /// <summary>
    ///     Lists tags for the edit.
    /// </summary>
    public bool IncludeTags { get; set; }

    /// <summary>
    ///     Show only items that are new.
    /// </summary>
    public PropertyFilterOption NewFilter { get; set; }

    /// <summary>
    ///     Show only items that are auto patrolled.
    /// </summary>
    public PropertyFilterOption AutoPatrolledFilter { get; set; }

    /// <summary>
    ///     Show only items that are minor.
    /// </summary>
    public PropertyFilterOption MinorFilter { get; set; }

    /// <summary>
    ///     Show only items that are patrolled.
    /// </summary>
    public PropertyFilterOption PatrolledFilter { get; set; }

    /// <summary>
    ///     Show only items that top.
    /// </summary>
    public PropertyFilterOption TopFilter { get; set; }

    /// <summary>
    ///     Only list revisions tagged with this tag.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    ///     In which direction to enumerate:
    /// </summary>
    public bool OrderDescending { get; set; }

    /// <summary>
    ///     Only list contributions in these namespaces.
    /// </summary>
    public int? NamespaceId { get; set; }

    public override IEnumerable<KeyValuePair<string, object?>> EnumListParameters()
    {
        var request = new Dictionary<string, object?>
        {
            {"ucstart", StartTime},
            {"ucend", EndTime},
            {"ucprop", PrepareUcProp()},
            {"ucshow", PrepareUcShow()},
            {"ucdir", OrderDescending ? "newer" : "older"},
            {"uctag", Tag},
            {"uclimit", PaginationSize},
            {"ucnamespace", NamespaceId}
        };
        if (_usernames != null)
        {
            request.Add("ucuser", string.Join('|', _usernames));
        }
        else if (_userIds != null)
        {
            request.Add("ucuserids", string.Join('|', _userIds));
        }
        else if (_userPrefix != null)
        {
            request.Add("ucuserprefix", _userPrefix);
        }

        return request;
    }

    protected override UserContributionResultItem ItemFromJson(JToken json)
    {
        JToken? title = json["title"];
        if (title is null)
        {
            throw new Exception("Title field is missing from json");
        }

        var wikiPage = new WikiPage(Site, title.Value<string>() ?? string.Empty);
        MediaWikiHelper.PopulatePageFromJson(wikiPage, (JObject) json, new WikiPageQueryProvider
        {
            Properties = []
        });
        return new UserContributionResultItem(wikiPage);
    } // ReSharper disable EnforceIfStatementBraces
    private string PrepareUcProp()
    {
        var props = new List<string>();
        props.AddIf("ids", IncludeIds);
        props.AddIf("title", IncludeTitle);
        props.AddIf("timestamp", IncludeTimestamp);
        props.AddIf("comment", IncludeComment);
        props.AddIf("parsedcomment", IncludeParsedComment);
        props.AddIf("size", IncludeSize);
        props.AddIf("sizediff", IncludeSizeDiff);
        props.AddIf("flags", IncludeFlags);
        props.AddIf("patrolled", IncludePatrolled);
        props.AddIf("tags", IncludeTags);
        return string.Join('|', props);
    }

    private string PrepareUcShow()
    {
        var props = new List<string?>
        {
            NewFilter.ToString("new", "!new", null),
            AutoPatrolledFilter.ToString("autopatrolled", "!autopatrolled", null),
            MinorFilter.ToString("minor", "!minor", null),
            PatrolledFilter.ToString("patrolled", "!patrolled", null),
            TopFilter.ToString("top", "!top", null)
        };
        return string.Join('|', props.Where(p => p != null));
    }
}
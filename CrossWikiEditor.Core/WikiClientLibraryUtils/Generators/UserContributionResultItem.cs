using Newtonsoft.Json;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

public sealed class UserContributionResultItem(WikiPage wikiPage)
{
    public WikiPage WikiPage { get; } = wikiPage;

    [JsonProperty("userid")]
    public int UserId { get; set; }
    
    [JsonProperty("user")]
    public string UserName { get; set; }
    
    [JsonProperty]
    public int Pageid { get; set; }
    
    [JsonProperty("revid")]
    public int Revid { get; set; }
    
    [JsonProperty("parentid")]
    public int ParentId { get; set; }
    
    /// <summary>
    /// Namespace id of the page.
    /// </summary>
    [JsonProperty("ns")]
    public string NamespaceId { get; set; }
    
    [JsonProperty]
    public string Title { get; set; }
    
    [JsonProperty]
    public string timestamp { get; set; }
    
    [JsonProperty("new")]
    public bool IsNew { get; set; }
    
    [JsonProperty("minor")]
    public bool IsMinor { get; set; }
    
    [JsonProperty("top")]
    public bool IsTop { get; set; }
    
    [JsonProperty]
    public string Comment { get; set; }
    
    [JsonProperty("parsedcomment")]
    public string ParsedComment { get; set; }
    
    [JsonProperty]
    public bool Patrolled { get; set; }
    
    [JsonProperty("autopatrolled")]
    public bool AutoPatrolled { get; set; }
    
    [JsonProperty("size")]
    public int ContentLength { get; set; }
    
    [JsonProperty("sizediff")]
    public int ContentLengthDiff { get; set; }
    
    [JsonProperty("tags")]
    public string[] Tags { get; set; }
}


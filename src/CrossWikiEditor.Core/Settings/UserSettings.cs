namespace CrossWikiEditor.Core.Settings;

public sealed class UserSettings
{
    public UserWiki UserWiki { get; set; } = new UserWiki("hy", ProjectEnum.Wikipedia);
    public NormalFindAndReplaceRules NormalFindAndReplaceRules { get; set; } = [];
    public bool IsBotMode { get; set; }
    public string DefaultSummary { get; set; } = string.Empty;
    public string GetApiUrl() => UserWiki.GetApiUrl();
    public string GetBaseUrl() => UserWiki.GetBaseUrl();
    public string GetLongBaseUrl() => UserWiki.GetLongBaseUrl();
    public string GetIndexUrl() => UserWiki.GetIndexUrl();
}
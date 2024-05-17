namespace CrossWikiEditor.Core.Settings;

public sealed class UserSettings
{
    public UserWiki UserWiki { get; set; } = new UserWiki("hy", ProjectEnum.Wikipedia);
    public SkipOptions SkipOptions { get; set; } = new();
    public GeneralOptions GeneralOptions { get; set; } = new();
    public MoreOptions MoreOptions { get; set; } = new();
    public bool IsBotMode { get; set; }
    public string DefaultSummary { get; set; } = string.Empty;
    public string GetApiUrl() => UserWiki.GetApiUrl();
    public string GetBaseUrl() => UserWiki.GetBaseUrl();
    public string GetLongBaseUrl() => UserWiki.GetLongBaseUrl();
    public string GetIndexUrl() => UserWiki.GetIndexUrl();
    public static UserSettings GetDefaultUserSettings() => new(); // TODO: For now
}
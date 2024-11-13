namespace CrossWikiEditor.Core.Settings;

public sealed class UserSettings
{
    public UserWiki UserWiki { get; set; } = new("hy", ProjectEnum.Wikipedia);
    public SkipOptions SkipOptions { get; set; } = new();
    public GeneralOptions GeneralOptions { get; set; } = new();
    public MoreOptions MoreOptions { get; set; } = new();
    public bool IsBotMode { get; set; }
    public string DefaultSummary { get; set; } = string.Empty;

    public string GetApiUrl()
    {
        return UserWiki.GetApiUrl();
    }

    public string GetBaseUrl()
    {
        return UserWiki.GetBaseUrl();
    }

    public string GetLongBaseUrl()
    {
        return UserWiki.GetLongBaseUrl();
    }

    public string GetIndexUrl()
    {
        return UserWiki.GetIndexUrl();
    }

    public static UserSettings GetDefaultUserSettings()
    {
        return new UserSettings();
        // TODO: For now
    }
}
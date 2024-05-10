using System.Text.Json;

namespace CrossWikiEditor.Core.Services;

public interface IUserPreferencesService
{
    UserSettings? GetUserSettings(string path);
    UserSettings GetCurrentSettings();
    void SetCurrentPref(UserSettings userSettings);
    string CurrentApiUrl { get; }
}

public sealed class UserPreferencesService : IUserPreferencesService
{
    private UserSettings _currentSettings;

    public UserPreferencesService(IMessengerWrapper messenger)
    {
        _currentSettings = new UserSettings();
        messenger.Register<LanguageCodeChangedMessage>(this, (r, m) => _currentSettings.UserWiki!.LanguageCode = m.Value);
        messenger.Register<ProjectChangedMessage>(this, (r, m) => _currentSettings.UserWiki!.Project = m.Value);
    }

    public UserSettings? GetUserSettings(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        string json = File.ReadAllText(path, Encoding.UTF8);
        return JsonSerializer.Deserialize<UserSettings>(json);
    }

    public UserSettings GetCurrentSettings()
    {
        return _currentSettings;
    }

    public string CurrentApiUrl => _currentSettings.UserWiki.GetApiUrl();

    public void SetCurrentPref(UserSettings userSettings)
    {
        _currentSettings = userSettings;
    }
}
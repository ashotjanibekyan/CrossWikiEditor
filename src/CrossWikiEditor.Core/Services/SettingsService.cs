using System.Text.Json;

namespace CrossWikiEditor.Core.Services;

public interface ISettingsService
{
    UserSettings GetDefaultSettings(bool force = false);
    UserSettings? GetUserSettingsByPath(string path);
    UserSettings GetCurrentSettings();
    void SaveCurrentUserSettings();
    void SetCurrentUserSettings(UserSettings userSettings);
    string CurrentApiUrl { get; }
}

public sealed class SettingsService : ISettingsService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private UserSettings _currentSettings;
    
    public SettingsService(IMessengerWrapper messenger)
    {
        _currentSettings = new UserSettings();
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        messenger.Register<LanguageCodeChangedMessage>(this, (r, m) => _currentSettings.UserWiki!.LanguageCode = m.Value);
        messenger.Register<ProjectChangedMessage>(this, (r, m) => _currentSettings.UserWiki!.Project = m.Value);
    }

    public UserSettings GetDefaultSettings(bool force = false)
    {
        if (!force && _currentSettings is not null)
        {
            return _currentSettings;
        }
        if (!File.Exists("./settings.json"))
        {
            return new UserSettings();
        }
        var json = File.ReadAllText("./settings.json");
        _currentSettings = JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
        return _currentSettings;
    }

    public UserSettings? GetUserSettingsByPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        string json = File.ReadAllText(path, Encoding.UTF8);
        return JsonSerializer.Deserialize<UserSettings>(json);
    }


    public void SaveCurrentUserSettings()
    {
        var settings = new UserSettings()
        {
            NormalFindAndReplaceRules = MainWindowViewModel.Instance!.OptionsViewModel.NormalFindAndReplaceRules
        };
        var json = JsonSerializer.Serialize(settings, options: _jsonSerializerOptions);
        File.WriteAllText("./settings.json", json);
    }

    public UserSettings GetCurrentSettings()
    {
        return _currentSettings;
    }

    public string CurrentApiUrl => _currentSettings.UserWiki.GetApiUrl();

    public void SetCurrentUserSettings(UserSettings userSettings)
    {
        _currentSettings = userSettings;
    }
}
using System.Text.Json;

namespace CrossWikiEditor.Core.Services;

public interface ISettingsService
{
    string CurrentApiUrl { get; }
    UserSettings GetDefaultSettings();
    UserSettings? GetSettingsByPath(string path);
    UserSettings GetCurrentSettings();
    void SaveCurrentSettings();
    void SetCurrentSettings(UserSettings userSettings);
}

public sealed class SettingsService : ISettingsService
{
    private readonly string _currentSettingsPath;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IMessengerWrapper _messenger;
    private UserSettings _currentSettings;

    public SettingsService(IMessengerWrapper messenger)
    {
        _messenger = messenger;
        _currentSettingsPath = "./settings.json";
        if (File.Exists(_currentSettingsPath))
        {
            UserSettings? temp = GetSettingsByPath(_currentSettingsPath);
            if (temp is not null)
            {
                _currentSettings = temp;
            }
        }

        _currentSettings ??= GetDefaultSettings();
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        messenger.Register<LanguageCodeChangedMessage>(this, (r, m) => _currentSettings.UserWiki!.LanguageCode = m.Value);
        messenger.Register<ProjectChangedMessage>(this, (r, m) => _currentSettings.UserWiki!.Project = m.Value);
    }

    public UserSettings GetDefaultSettings()
    {
        return UserSettings.GetDefaultUserSettings();
    }

    public UserSettings? GetSettingsByPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        string json = File.ReadAllText(path, Encoding.UTF8);
        return JsonSerializer.Deserialize<UserSettings>(json);
    }

    public void SaveCurrentSettings()
    {
        if (File.Exists(_currentSettingsPath))
        {
            if (!Directory.Exists("./oldSettings"))
            {
                Directory.CreateDirectory("./oldSettings");
            }

            // Just in case. This is just a json file, so no big deal, they can delete it themself.
            File.Move(_currentSettingsPath, $"./oldSettings/{DateTime.Now:yyyyMMdd_HHmmss}_settings.json");
        }

        string? json = JsonSerializer.Serialize(_currentSettings, _jsonSerializerOptions);
        File.WriteAllText(_currentSettingsPath, json);
    }

    public UserSettings GetCurrentSettings()
    {
        return _currentSettings;
    }

    public string CurrentApiUrl => _currentSettings.UserWiki.GetApiUrl();

    public void SetCurrentSettings(UserSettings userSettings)
    {
        _currentSettings = userSettings;
        _messenger.Send(new CurrentSettingsUpdatedMessage(userSettings));
    }
}
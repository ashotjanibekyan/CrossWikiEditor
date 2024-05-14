using System.Text.Json;
using CommunityToolkit.Mvvm.Messaging;

namespace CrossWikiEditor.Core.Services;

public interface ISettingsService
{
    UserSettings GetDefaultSettings();
    UserSettings? GetSettingsByPath(string path);
    UserSettings GetCurrentSettings();
    void SaveCurrentSettings();
    void SetCurrentSettings(UserSettings userSettings);
    string CurrentApiUrl { get; }
}

public sealed class SettingsService : ISettingsService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IMessengerWrapper _messenger;
    private string _currentSettingsPath;
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

    public UserSettings GetDefaultSettings() => UserSettings.GetDefaultUserSettings();

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
        var json = JsonSerializer.Serialize(_currentSettings, options: _jsonSerializerOptions);
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
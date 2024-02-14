using System.Text.Json;

namespace CrossWikiEditor.Core.Services;

public interface ISettingsService
{
    void SaveCurrentSettings();
    Settings.Settings GetDefaultSettings(bool force = false);
}

public sealed class SettingsService() : ISettingsService
{
    private Settings.Settings? _settings;
    public void SaveCurrentSettings()
    {
        var settings = new Settings.Settings()
        {
            NormalFindAndReplaceRules = MainWindowViewModel.Instance!.OptionsViewModel.NormalFindAndReplaceRules
        };
        var json = JsonSerializer.Serialize(settings, options: new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText("./settings.json", json);
    }

    public Settings.Settings GetDefaultSettings(bool force = false)
    {
        if (!force && _settings is not null)
        {
            return _settings;
        }
        if (!File.Exists("./settings.json"))
        {
            return new Settings.Settings();
        }
        var json = File.ReadAllText("./settings.json");
        _settings = JsonSerializer.Deserialize<Settings.Settings>(json);
        return _settings ?? new Settings.Settings();
    }
}
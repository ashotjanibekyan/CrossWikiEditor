using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.Services;

public interface IUserPreferencesService
{
    UserPrefs GetUserPref(string path);
    UserPrefs GetCurrentPref();
    string CurrentApiUrl { get; }
    void SetCurrentPref(UserPrefs userPrefs);
}

public sealed class UserPreferencesService : IUserPreferencesService
{
    private UserPrefs _currentPref;

    public UserPreferencesService(IMessengerWrapper messenger)
    {
        _currentPref = new UserPrefs();
        messenger.Register<LanguageCodeChangedMessage>(this, (r, m) => _currentPref.LanguageCode = m.Value);
        messenger.Register<ProjectChangedMessage>(this, (r, m) => _currentPref.Project = m.Value);
    }

    public UserPrefs GetUserPref(string path)
    {
        string settings = File.ReadAllText(path, Encoding.UTF8);
        settings = Regex.Replace(settings, @"<(/?)\s*SourceIndex>", "<$1SelectedProvider>");
        var xs = new XmlSerializer(typeof(UserPrefs));
        return (UserPrefs) (xs.Deserialize(new StringReader(settings)) ?? throw new InvalidOperationException());
    }

    public string CurrentApiUrl => _currentPref.UrlApi();

    public void SetCurrentPref(UserPrefs userPrefs)
    {
        _currentPref = userPrefs;
    }

    public UserPrefs GetCurrentPref()
    {
        return _currentPref;
    }
}
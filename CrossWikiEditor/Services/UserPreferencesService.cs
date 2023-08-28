using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.Messages;

namespace CrossWikiEditor.Services;

public interface IUserPreferencesService
{
    UserPrefs GetUserPref(string path);
    UserPrefs GetCurrentPref();
    void SetCurrentPref(UserPrefs userPrefs);
}

public sealed class UserPreferencesService : IUserPreferencesService
{
    private UserPrefs _currentPref;

    public UserPreferencesService(IMessenger messenger)
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

    public void SetCurrentPref(UserPrefs userPrefs)
    {
        _currentPref = userPrefs;
    }

    public UserPrefs GetCurrentPref()
    {
        return _currentPref;
    }
}
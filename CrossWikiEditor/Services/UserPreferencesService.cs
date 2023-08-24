﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using CrossWikiEditor.Messages;
using ReactiveUI;

namespace CrossWikiEditor.Services;

public interface IUserPreferencesService
{
    UserPrefs GetUserPref(string path);
    UserPrefs GetCurrentPref();
}

public sealed class UserPreferencesService : IUserPreferencesService
{
    private UserPrefs _currentPref;

    public UserPreferencesService()
    {
        _currentPref = new UserPrefs();

        MessageBus.Current.Listen<LanguageCodeChangedMessage>()
            .Subscribe(m => _currentPref.LanguageCode = m.LanguageCode);
        MessageBus.Current.Listen<ProjectChangedMessage>()
            .Subscribe(m => _currentPref.Project = m.Project);
    }
    
    public UserPrefs GetUserPref(string path)
    {
        var settings = File.ReadAllText(path, Encoding.UTF8);
        settings = Regex.Replace(settings, @"<(/?)\s*SourceIndex>", "<$1SelectedProvider>");
        var xs = new XmlSerializer(typeof(UserPrefs));
        return (UserPrefs)xs.Deserialize(new StringReader(settings));
    }

    public UserPrefs GetCurrentPref()
    {
        return _currentPref;
    }
}
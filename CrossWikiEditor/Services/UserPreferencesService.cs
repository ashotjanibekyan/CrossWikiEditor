using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CrossWikiEditor.Services;

public interface IUserPreferencesService
{
    UserPrefs GetUserPref(string path);
}


public class UserPreferencesService : IUserPreferencesService
{
    public UserPrefs GetUserPref(string path)
    {
        string settings = File.ReadAllText(path, Encoding.UTF8);


        settings = Regex.Replace(settings, @"<(/?)\s*SourceIndex>", "<$1SelectedProvider>");

        XmlSerializer xs = new XmlSerializer(typeof(UserPrefs));
        return (UserPrefs)xs.Deserialize(new StringReader(settings));
    }
}
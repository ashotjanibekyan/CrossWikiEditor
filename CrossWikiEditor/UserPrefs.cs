using System;
using System.Linq;
using System.Xml.Serialization;

namespace CrossWikiEditor;

[Serializable, XmlRoot("AutoWikiBrowserPreferences")]
public struct UserPrefs
{

    [XmlAttribute("xml:space")]
    public string SpacePreserve = "preserve";

    public UserPrefs()
    {
        Version = "0.0.1";
        Project = ProjectEnum.Wikipedia;
        LanguageCode = "en";
        CustomProject = "";
        Protocol = "http://";
        LoginDomain = "";
    }

    public UserPrefs(string version, ProjectEnum project, string languageCode, string customProject, string protocol, string loginDomain)
    {
        Version = version;
        Project = project;
        LanguageCode = languageCode;
        CustomProject = customProject;
        Protocol = protocol;
        LoginDomain = loginDomain;
    }

    [XmlAttribute]
    public string Version { get; set; }
    public ProjectEnum Project { get; set; }
    public string LanguageCode { get; set; }
    public string CustomProject { get; set; }
    public string Protocol { get; set; }
    public string LoginDomain { get; set; }

    public string ApiRoot()
    {
        if (!string.IsNullOrEmpty(LanguageCode) && new[]
            {
                ProjectEnum.Wikipedia,
                ProjectEnum.Wiktionary,
                ProjectEnum.Wikisource,
                ProjectEnum.Wikiquote,
                ProjectEnum.Wikiversity,
                ProjectEnum.Wikivoyage,
                ProjectEnum.Wikibooks,
                ProjectEnum.Wikinews
            }.Contains(Project))
        {
            return $"https://{LanguageCode}.{Project.ToString().ToLower()}.org/w/api.php?";
        }

        throw new NotImplementedException();
    }
}
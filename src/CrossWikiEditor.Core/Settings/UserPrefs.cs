namespace CrossWikiEditor.Core.Settings;

[Serializable]
[XmlRoot("AutoWikiBrowserPreferences")]
public struct UserPrefs(string version, ProjectEnum project, string languageCode, string customProject, string protocol, string loginDomain)
{
    [XmlAttribute("xml:space")] public string SpacePreserve = "preserve";

    public UserPrefs() : this("0.0.1", ProjectEnum.Wikipedia, "hy", "", "http://", "")
    {
    }

    [XmlAttribute] public string Version { get; set; } = version;
    public ProjectEnum Project { get; set; } = project;
    public string LanguageCode { get; set; } = languageCode;
    public string CustomProject { get; set; } = customProject;
    public string Protocol { get; set; } = protocol;
    public string LoginDomain { get; set; } = loginDomain;

    public string UrlBase()
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
            return $"https://{LanguageCode}.{Project.ToString().ToLower()}.org";
        }

        throw new NotImplementedException();
    }

    public string UrlBaseLong()
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
            return $"https://{LanguageCode}.{Project.ToString().ToLower()}.org/w/";
        }

        throw new NotImplementedException();
    }

    public string UrlApi()
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

    public string UrlIndex()
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
            return $"https://{LanguageCode}.{Project.ToString().ToLower()}.org/w/index.php?";
        }

        throw new NotImplementedException();
    }
}
namespace CrossWikiEditor.Core.Settings;

public sealed class UserWiki(string? languageCode, ProjectEnum project)
{
    public string? LanguageCode { get; set; } = languageCode;
    public ProjectEnum Project { get; set; } = project;

    public string GetBaseUrl()
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

    public string GetLongBaseUrl()
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

    public string GetApiUrl()
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

    public string GetIndexUrl()
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
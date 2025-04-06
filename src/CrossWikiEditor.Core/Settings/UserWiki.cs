using System;
using System.Linq;

namespace CrossWikiEditor.Core.Settings;

public sealed class UserWiki
{
    public UserWiki(string? languageCode, ProjectEnum project)
    {
        LanguageCode = languageCode;
        Project = project;
    }

    public string? LanguageCode { get; set; }
    public ProjectEnum Project { get; set; }

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
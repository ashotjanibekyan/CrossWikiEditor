using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Settings;

namespace CrossWikiEditor.Tests.ListProviders;

public class ListProvidersBaseTest : BaseTest
{
    protected UserPrefs _userPrefs;
    
    protected void SetUpUserPrefs(string languageCode, ProjectEnum project)
    {
        _userPrefs = new UserPrefs
        {
            LanguageCode = languageCode,
            Project = project
        };
        _userPreferencesService.GetCurrentPref().Returns(_userPrefs);
    }
}
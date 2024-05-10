using System.Text.RegularExpressions;

namespace CrossWikiEditor.Tests.Utils;

public sealed class LanguageSpecificRegexesTests : BaseTest
{
    private LanguageSpecificRegexes _sut;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        SetUpServices();
        string apiRoot = "https://en.wikipedia.org/w/api.php?";
        _userPreferencesService.CurrentApiUrl.Returns(apiRoot);
        _userPreferencesService.GetCurrentSettings().Returns(new UserSettings()
        {
            UserWiki = new("en", ProjectEnum.Wikipedia)
        });
        var enWiki = new WikiSite(new WikiClient(), apiRoot);
        await enWiki.Initialization;
        _wikiClientCache.GetWikiSite(apiRoot).Returns(enWiki);
        _sut = new LanguageSpecificRegexes(_userPreferencesService, _wikiClientCache, _messenger);
        await _sut.InitAsync;
    }
    
    [Test]
    public void ExtractTitleTests()
    {
        if (_sut.ExtractTitle is null)
        {
            Assert.Fail("LanguageSpecificRegexes isn't initialized properly");
            return;
        }
        
        IsMatch(_sut.ExtractTitle, @"https://en.wikipedia.org/wiki/Foo");
        IsMatch(_sut.ExtractTitle, @"https://en.wikipedia.org/wiki/Foo_bar");
            
        Assert.That(_sut.ExtractTitle.Match(@"https://en.wikipedia.org/wiki/Foo").Groups[1].Value, Is.EqualTo("Foo"));
        Assert.That(_sut.ExtractTitle.Match(@"https://en.wikipedia.org/w/index.php?title=Foo").Groups[1].Value, Is.EqualTo("Foo"));
        Assert.That(_sut.ExtractTitle.Match(@"https://en.wikipedia.org/w/index.php/Foo").Groups[1].Value, Is.EqualTo("Foo"));
        Assert.That(_sut.ExtractTitle.Match(@"https://en.wikipedia.org/w/index.php/Foo bar here").Groups[1].Value, Is.EqualTo("Foo bar here"));
            
        NoMatch(_sut.ExtractTitle, @"https://random.org/wiki/Foo");
        NoMatch(_sut.ExtractTitle, @"https://en.wikipedia.org/wikirandom/Foo");
        NoMatch(_sut.ExtractTitle, @"https://hy.wikipedia.org/wiki/Foo");
        NoMatch(_sut.ExtractTitle, @"https://hy.wikipedia.org/wiki/Foo_bar");
    }

    private static void IsMatch(Regex regex, string input)
    {
        if (!regex.IsMatch(input))
        {
            throw new AssertionException($"The string <{input}> does not match the given regex <{regex}>");
        }
    }

    private static void NoMatch(Regex regex, string input)
    {
        Assert.That(regex.IsMatch(input), Is.False, "The string matches the given regex");
    }
}
using CrossWikiEditor.Core.Services.WikiServices;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor.Tests.Utils;

public class WikiPageExtensionsTests : BaseTest
{
    private const string ApiRoot = "https://hy.wikipedia.org/w/api.php?";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SetUpServices();
        _wikiClientCache = new WikiClientCache(_logger);
    }

    [TestCase("Քննարկում:Վերնագիր")]
    [TestCase("Մասնակցի քննարկում:Վերնագիր")]
    [TestCase("Վիքիպեդիայի քննարկում:Վերնագիր")]
    [TestCase("Կաղապարի քննարկում:Վերնագիր")]
    [TestCase("MediaWiki քննարկում:Վերնագիր")]
    public async Task ToTalkPage_ShouldReturnSamePage_WhenPageIsTalk(string talkPageTitle)
    {
        // arrange
        var wikiPage = new WikiPage(await _wikiClientCache.GetWikiSite(ApiRoot), talkPageTitle);

        // act
        WikiPage talkWikiPage = wikiPage.ToTalkPage();

        // assert
        talkWikiPage.Should().Be(wikiPage);
    }

    [TestCase("Վերնագիր", "Քննարկում:Վերնագիր")]
    [TestCase("Մասնակից:Վերնագիր", "Մասնակցի քննարկում:Վերնագիր")]
    [TestCase("Վիքիպեդիա:Վերնագիր", "Վիքիպեդիայի քննարկում:Վերնագիր")]
    [TestCase("Կաղապար:Վերնագիր", "Կաղապարի քննարկում:Վերնագիր")]
    [TestCase("MediaWiki:Վերնագիր", "MediaWiki քննարկում:Վերնագիր")]
    public async Task ToTalkPage_ShouldReturnTalkPage_WhenPageIsSubject(string pageTitle, string talkPageTitle)
    {
        // arrange
        var page = new WikiPage(await _wikiClientCache.GetWikiSite(ApiRoot), pageTitle);
        var expectedTalkPage = new WikiPage(await _wikiClientCache.GetWikiSite(ApiRoot), talkPageTitle);

        // act
        WikiPage talkWikiPage = page.ToTalkPage();

        // assert
        talkWikiPage.Should().BeEquivalentTo(expectedTalkPage);
    }

    [TestCase("Վերնագիր")]
    [TestCase("Մասնակից:Վերնագիր")]
    [TestCase("Վիքիպեդիա:Վերնագիր")]
    [TestCase("Կաղապար:Վերնագիր\"")]
    [TestCase("MediaWiki:Վերնագիր")]
    public async Task ToSubjectPage_ShouldReturnSamePage_WhenPageIsSubject(string subjectPageTitle)
    {
        // arrange
        var wikiPage = new WikiPage(await _wikiClientCache.GetWikiSite(ApiRoot), subjectPageTitle);

        // act
        WikiPage subjectWikiPage = wikiPage.ToSubjectPage();

        // assert
        subjectWikiPage.Should().Be(wikiPage);
    }

    [TestCase("Վերնագիր", "Քննարկում:Վերնագիր")]
    [TestCase("Մասնակից:Վերնագիր", "Մասնակցի քննարկում:Վերնագիր")]
    [TestCase("Վիքիպեդիա:Վերնագիր", "Վիքիպեդիայի քննարկում:Վերնագիր")]
    [TestCase("Կաղապար:Վերնագիր", "Կաղապարի քննարկում:Վերնագիր")]
    [TestCase("MediaWiki:Վերնագիր", "MediaWiki քննարկում:Վերնագիր")]
    public async Task ToSubjectPage_ShouldReturnTalkPage_WhenPageIsSubject(string pageTitle, string talkPageTitle)
    {
        // arrange
        var page = new WikiPage(await _wikiClientCache.GetWikiSite(ApiRoot), talkPageTitle);
        var expectedSubjectPage = new WikiPage(await _wikiClientCache.GetWikiSite(ApiRoot), pageTitle);

        // act
        WikiPage subjectWikiPage = page.ToSubjectPage();

        // assert
        subjectWikiPage.Should().BeEquivalentTo(expectedSubjectPage);
    }
}
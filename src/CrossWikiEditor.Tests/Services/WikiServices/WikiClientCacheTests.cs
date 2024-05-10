using CrossWikiEditor.Core.Services.WikiServices;

namespace CrossWikiEditor.Tests.Services.WikiServices;

public sealed class WikiClientCacheTests : BaseTest
{
    private WikiClientCache _sut;
    private const string ApiRoot = "https://hy.wikipedia.org/w/api.php?";
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new WikiClientCache(_logger);
    }

    [Test]
    public void GetWikiClient_ShouldReturnNewWikiClient_WhenCacheIsEmpty()
    {
        // arrange

        // act
        WikiClient client = _sut.GetWikiClient(ApiRoot);

        // assert
        client.Should().NotBeNull();
    }

    [Test]
    public void GetWikiClient_ShouldReturnCachedWikiClient_WhenCacheIsNotEmpty()
    {
        // arrange
        WikiClient firstClient = _sut.GetWikiClient(ApiRoot);

        // act
        WikiClient secondClient = _sut.GetWikiClient(ApiRoot);

        // assert
        firstClient.Should().Be(secondClient);
    }

    [Test]
    public void GetWikiClient_ShouldReturnNewWikiClient_WhenCacheIsNotEmptyAndForceNewIsTrue()
    {
        // arrange
        WikiClient firstClient = _sut.GetWikiClient(ApiRoot);

        // act
        WikiClient secondClient = _sut.GetWikiClient(ApiRoot, true);

        // assert
        firstClient.Should().NotBe(secondClient);
    }

    [Test]
    public async Task GetSiteClient_ShouldReturnNewWikiSite_WhenCacheIsEmpty()
    {
        // arrange

        // act
        WikiSite site = await _sut.GetWikiSite(ApiRoot);

        // assert
        site.Should().NotBeNull();
    }

    [Test]
    public async Task GetWikiSite_ShouldReturnCachedWikiSite_WhenCacheIsNotEmpty()
    {
        // arrange
        WikiSite firstSite = await _sut.GetWikiSite(ApiRoot);

        // act
        WikiSite secondSite = await _sut.GetWikiSite(ApiRoot);

        // assert
        firstSite.Should().Be(secondSite);
    }

    [Test]
    public async Task GetWikiSite_ShouldReturnNewWikiSite_WhenCacheIsNotEmptyAndForceNewIsTrue()
    {
        // arrange
        WikiSite firstSite = await _sut.GetWikiSite(ApiRoot);

        // act
        WikiSite secondSite = await _sut.GetWikiSite(ApiRoot, true);

        // assert
        firstSite.Should().NotBe(secondSite);
    }

    [Test]
    public async Task GetWikiPageModel_ShouldReturnNewWikiPageModel_WhenForValidInput()
    {
        // arrange

        // act
        Result<WikiPageModel> result = await _sut.GetWikiPageModel(ApiRoot, "Վիքիպեդիա:Խորհրդարան");

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("Վիքիպեդիա:Խորհրդարան");
        result.Value.NamespaceId.Should().Be(4);
    }

    [Test]
    public async Task GetWikiPageModel_ShouldReturnFailedResult_WhenForInvalidApi()
    {
        // arrange

        // act
        Result<WikiPageModel> result = await _sut.GetWikiPageModel("this is not a valid url", "Վիքիպեդիա:Խորհրդարան");

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be("An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.");
    }

    [Test]
    public async Task GetWikiPageModel_ShouldReturnFailedResult_WhenForInvalidTitle()
    {
        // arrange

        // act
        Result<WikiPageModel> result = await _sut.GetWikiPageModel(ApiRoot, "Վիք[{]իպեդիա:Խորհրդարան");

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be("Title contains illegal character sequence: [ .");
    }
}
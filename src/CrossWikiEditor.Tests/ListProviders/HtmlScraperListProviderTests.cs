using System.Net;
using CrossWikiEditor.Core.Services.HtmlParsers;
using RichardSzalay.MockHttp;

namespace CrossWikiEditor.Tests.ListProviders;

public sealed class HtmlScraperListProviderTests : ListProvidersBaseTest<HtmlScraperListProvider>
{
    private IHttpClientFactory _httpClientFactory;
    private MockHttpMessageHandler _mockHttpMessageHandler;

    private readonly string _htmlWithDuplicateLinks = """
        <html>
            <body>
                <a href="https://hy.wikipedia.org/wiki/Kotlin" >Kotlin programming language</a>
                <a href="https://hy.wikipedia.org/wiki/%D5%8C%D5%A5%D5%B8_%D5%A4%D5%B2%D5%B5%D5%A1%D5%AF" >Ռեո դղյակ</a>
                <a href="https://hy.wikipedia.org/wiki/%D5%8C%D5%A5%D5%B8_%D5%A4%D5%B2%D5%B5%D5%A1%D5%AF" >Ռեո դղյակ</a>
                <a href="https://hy.wikipedia.org/wiki/%D5%8C%D5%A5%D5%B8_%D5%A4%D5%B2%D5%B5%D5%A1%D5%AF" >Ռեո դղյակ</a>
                <a href="https://hy.wikipedia.org/wiki/%D5%8C%D5%A5%D5%B8_%D5%A4%D5%B2%D5%B5%D5%A1%D5%AF" >Ռեո դղյակ</a>
                <a href="https://hy.wikipedia.org/wiki/%D5%8C%D5%A5%D5%B8_%D5%A4%D5%B2%D5%B5%D5%A1%D5%AF" >Ռեո դղյակ</a>
            </body>
        </html>
        """;

    private readonly string _htmlWithoutLinks = """
        <html>
            <body>
            </body>
        </html>
        """;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hy", ProjectEnum.Wikipedia);
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        _sut = new HtmlScraperListProvider(new HtmlAgilityPackParser(_logger, _userPreferencesService, _wikiClientCache), _httpClientFactory, _logger,
            new SimpleHtmlParser(_logger, _userPreferencesService, _wikiClientCache));
    }

    [Test] public new void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty();
    [Test] public new void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty();

    [Test]
    public async Task MakeList_ShouldReturnFailure_WhenRequestIsNotSuccessful()
    {
        // arrange
        _sut.Param = "https://en.wikipedia.org/wiki/Magic_string";
        _httpClientFactory.CreateClient("Scraper").Returns(new HttpClient(_mockHttpMessageHandler));
        _mockHttpMessageHandler
            .When(_sut.Param)
        .Respond(HttpStatusCode.Found, new StringContent("error message"));
        
        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Response status code does not indicate success: 302 (Found).");
    }
    
    [Test]
    public async Task MakeList_ShouldReturnUniquePages_WhenHtmlContainsLinks()
    {
        // arrange
        _sut.Param = "https://en.wikipedia.org/wiki/Magic_string";
        _httpClientFactory.CreateClient("Scraper").Returns(new HttpClient(_mockHttpMessageHandler));
        _mockHttpMessageHandler
            .When(_sut.Param)
            .Respond("application/text", _htmlWithDuplicateLinks);
        
        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value![0].Title.Should().Be("Kotlin");
        result.Value[1].Title.Should().Be("Ռեո դղյակ");
    }
    
    [Test]
    public async Task MakeList_ShouldReturnEmptyList_WhenHtmlDoesNotContainsLinks()
    {
        // arrange
        _sut.Param = "https://en.wikipedia.org/wiki/Magic_string";
        _httpClientFactory.CreateClient("Scraper").Returns(new HttpClient(_mockHttpMessageHandler));
        _mockHttpMessageHandler
            .When(_sut.Param)
            .Respond("application/text", _htmlWithoutLinks);
        
        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }
    
    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("HTML Scraper");
        _sut.ParamTitle.Should().Be("URL");
        _mockHttpMessageHandler?.Dispose();
    }
}
using CrossWikiEditor.Core.Services.HtmlParsers;
using RichardSzalay.MockHttp;

namespace CrossWikiEditor.Tests.ListProviders;

public sealed class HtmlScraperListProviderTests : ListProvidersBaseTest<HtmlScraperListProvider>
{
    private IHttpClientFactory _httpClientFactory;
    private MockHttpMessageHandler _mockHttpMessageHandler;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        _sut = new HtmlScraperListProvider(new HtmlAgilityPackParser(_logger, _userPreferencesService, _wikiClientCache), _httpClientFactory, _logger,
            new SimpleHtmlParser(_logger, _userPreferencesService, _wikiClientCache));
    }

    [Test] public new void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty();
    [Test] public new void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty();

    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("HTML Scraper");
        _sut.ParamTitle.Should().Be("URL");
    }
}
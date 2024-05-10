using System.Net;
using RichardSzalay.MockHttp;

namespace CrossWikiEditor.Tests.ListProviders;

public sealed class PetscanListProviderTests : ListProvidersBaseTest<PetscanListProvider>
{
    private IHttpClientFactory _httpClientFactory;
    private MockHttpMessageHandler _mockHttpMessageHandler;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        _sut = new PetscanListProvider(_httpClientFactory, _userPreferencesService, _wikiClientCache);
    }

    [Test]
    public void CanMake_ShouldBeTrue_WhenParamIsNotNull()
    {
        // arrange
        _sut.Param = "not empty";

        // act

        // assert
        _sut.CanMake.Should().BeTrue();
    }

    [Test]
    public void CanMake_ShouldBeFalse_WhenParamIsNull()
    {
        // arrange
        _sut.Param = "";

        // act

        // assert
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task MakeList_ShouldReturnFailure_WhenRequestThrowsException()
    {
        // arrange
        var id = 1234;
        _httpClientFactory.CreateClient("Petscan").Returns(new HttpClient(_mockHttpMessageHandler));
        _mockHttpMessageHandler
            .When($"https://petscan.wmflabs.org/?psid={id}&format=plain")
            .Throw(new Exception("exception message"));
        _sut.Param = id.ToString();

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("exception message");
    }
    
    [Test]
    public async Task MakeList_ShouldReturnFailure_WhenPetscanRequestIsNotSuccessful()
    {
        // arrange
        var id = 1234;
        _httpClientFactory.CreateClient("Petscan").Returns(new HttpClient(_mockHttpMessageHandler));
        _mockHttpMessageHandler
            .When($"https://petscan.wmflabs.org/?psid={id}&format=plain")
            .Respond(HttpStatusCode.Found, new StringContent("error message"));
        _sut.Param = id.ToString();

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Could not get the list. \nerror message");
    }

    [Test]
    public async Task MakeList_ShouldReturnFailure_WhenPSIDIsNotValid()
    {
        // arrange
        var id = "fwe";
        _httpClientFactory.CreateClient("Petscan").Returns(new HttpClient(_mockHttpMessageHandler));
        _mockHttpMessageHandler
            .When($"https://petscan.wmflabs.org/?psid={id}&format=plain")
            .Respond(HttpStatusCode.Found, new StringContent("error message"));
        _sut.Param = id;

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("fwe is not a valid PSID");
    }

    [Test]
    public async Task MakeList_ShouldFetchPlainTestFromPetscan()
    {
        // arrange
        var id = 1234;
        _httpClientFactory.CreateClient("Petscan").Returns(new HttpClient(_mockHttpMessageHandler));
        _mockHttpMessageHandler
            .When($"https://petscan.wmflabs.org/?psid={id}&format=plain")
            .Respond("application/text", "pagetitle1\npagetitle2\npagetitle3");
        _sut.Param = id.ToString();

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value![0].Title.Should().Be("pagetitle1");
        result.Value[1].Title.Should().Be("pagetitle2");
        result.Value[2].Title.Should().Be("pagetitle3");
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("Petscan");
        _sut.ParamTitle.Should().Be("PSID");
        _mockHttpMessageHandler?.Dispose();
    }
}
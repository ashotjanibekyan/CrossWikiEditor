using CrossWikiEditor.Core.Services.WikiServices;
using Serilog;

namespace CrossWikiEditor.Tests.ListProviders;

public class LinksOnPageRedListProviderTests : ListProvidersBaseTest
{
    private LinksOnPageRedListProvider _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _wikiClientCache = new WikiClientCache(Substitute.For<ILogger>());
        _selectNamespacesViewModel = new SelectNamespacesViewModel(new List<WikiNamespace>(), false);
        _sut = new LinksOnPageRedListProvider(_dialogService, _pageService, _userPreferencesService)
        {
            Param = "start from here"
        };
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
        _expectedPages.Add(new WikiPageModel("Գրիգոր Ամիրեանի Բնակելի Տուն", _userPrefs.UrlApi(), _wikiClientCache));
        _expectedPages.Add(new WikiPageModel("8 Յուլիս", _userPrefs.UrlApi(), _wikiClientCache));
    }
    
    [Test] public void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty(_sut);
    [Test] public void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty(_sut);
    
    [Test]
    public async Task MakeList_ShouldReturnBluePageServiceResults()
    {
        // arrange
        _pageService.LinksOnPage(_userPrefs.UrlApi(), _sut.Param, 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_sut, new List<WikiPageModel>
        {
            _expectedPages[0],
            _expectedPages[1],
            _expectedPages[2],
            _expectedPages[3],
        });
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.LinksOnPage(_userPrefs.UrlApi(), _sut.Param, 73)
            .Returns(Result<List<WikiPageModel>>.Failure("failed to get pages"));

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be("failed to get pages");
    }
    
    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("Links on page (only redlinks)");
        _sut.ParamTitle.Should().Be("Links on");
    }
}
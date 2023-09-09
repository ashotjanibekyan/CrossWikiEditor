namespace CrossWikiEditor.Tests.ListProviders;

public class PagesWithPropListProviderTests : ListProvidersBaseTest
{
    private PagesWithPropListProvider _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _selectNamespacesViewModel = new SelectNamespacesViewModel(new List<WikiNamespace>(), false);
        _sut = new PagesWithPropListProvider(_dialogService, _pageService, _userPreferencesService)
        {
            Param = "start from here"
        };
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
    }
    
    [Test] public void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty(_sut);
    [Test] public void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty(_sut);
    
    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _pageService.GetPagesWithProp(_userPrefs.UrlApi(), _sut.Param, 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_sut, _expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetPagesWithProp(_userPrefs.UrlApi(), _sut.Param, 73)
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
        _sut.Title.Should().Be("Pages with a page property");
        _sut.ParamTitle.Should().Be("Property name");
    }
}
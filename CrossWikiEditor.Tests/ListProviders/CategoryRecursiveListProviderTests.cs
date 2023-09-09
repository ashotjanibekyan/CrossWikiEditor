namespace CrossWikiEditor.Tests.ListProviders;

public class CategoryRecursiveListProviderTests : ListProvidersBaseTest
{
    private CategoryRecursiveListProvider _sut;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _sut = new CategoryRecursiveListProvider(_userPreferencesService, _categoryService, _dialogService);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
    }

    [Test] public void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty(_sut);
    [Test] public void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty(_sut);
        
    [Test]
    public async Task MakeList_ShouldReturnServiceResults()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userPreferencesService.CurrentApiUrl, _sut.Param, 73, int.MaxValue)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await base.MakeList_ShouldReturnServiceResults(_sut, _expectedPages);
    }
    
    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userPreferencesService.CurrentApiUrl, _sut.Param, 73, int.MaxValue)
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
        _sut.Title.Should().Be("Category (recursive)");
        _sut.ParamTitle.Should().Be("Category");
    }
}
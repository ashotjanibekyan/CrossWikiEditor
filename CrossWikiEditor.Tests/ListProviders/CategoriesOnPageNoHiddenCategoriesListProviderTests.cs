namespace CrossWikiEditor.Tests.ListProviders;

public sealed class CategoriesOnPageNoHiddenCategoriesListProviderTests : ListProvidersBaseTest<CategoriesOnPageNoHiddenCategoriesListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _sut = new CategoriesOnPageNoHiddenCategoriesListProvider(_categoryService, _dialogService, _userPreferencesService)
        {
            Param = "page"
        };
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
    }

    [Test] public new void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty();
    [Test] public new void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty();
        
    [Test]
    public async Task MakeList_ShouldReturnServiceResults()
    {
        // arrange
        _categoryService.GetCategoriesOf(_userPreferencesService.CurrentApiUrl, _sut.Param, 73, includeHidden: false)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await base.MakeList_ShouldReturnServiceResults(_expectedPages);
    }
    
    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _categoryService.GetCategoriesOf(_userPreferencesService.CurrentApiUrl, _sut.Param, 73, includeHidden: false)
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
        _sut.Title.Should().Be("Categories on page (no hidden categories)");
        _sut.ParamTitle.Should().Be("Page");
    }
}
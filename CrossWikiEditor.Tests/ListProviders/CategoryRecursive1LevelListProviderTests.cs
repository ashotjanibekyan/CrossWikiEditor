namespace CrossWikiEditor.Tests.ListProviders;

public sealed class CategoryRecursive1LevelListProviderTests : ListProvidersBaseTest<CategoryRecursive1LevelListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _sut = new CategoryRecursive1LevelListProvider(_categoryService, _dialogService, _userPreferencesService);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
    }

    [Test] public new void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty();
    [Test] public new void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty();
        
    [Test]
    public async Task MakeList_ShouldReturnServiceResults()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userPreferencesService.CurrentApiUrl, _sut.Param, 73, 1)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await base.MakeList_ShouldReturnServiceResults(_expectedPages);
    }
    
    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userPreferencesService.CurrentApiUrl, _sut.Param, 73, 1)
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
        _sut.Title.Should().Be("Category (recursive 1 level)");
        _sut.ParamTitle.Should().Be("Category");
    }
}
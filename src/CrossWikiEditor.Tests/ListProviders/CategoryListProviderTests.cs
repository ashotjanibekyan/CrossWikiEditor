namespace CrossWikiEditor.Tests.ListProviders;

public sealed class CategoryListProviderTests : ListProvidersBaseTest<CategoryListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _sut = new CategoryListProvider(_categoryService, _dialogService, _userPreferencesService);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test] public new void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty();
    [Test] public new void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty();
        
    [Test]
    public async Task MakeList_ShouldReturnServiceResults()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userPreferencesService.CurrentApiUrl, _sut.Param, 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await base.MakeList_ShouldReturnServiceResults(_expectedPages);
    }
    
    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userPreferencesService.CurrentApiUrl, _sut.Param, 73)
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
        _sut.Title.Should().Be("Category");
        _sut.ParamTitle.Should().Be("Category");
    }
}
namespace CrossWikiEditor.Tests.ListProviders;

public sealed class AllCategoriesListProviderTests : ListProvidersBaseTest<AllCategoriesListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _sut = new AllCategoriesListProvider(_categoryService, _dialogService, _userPreferencesService)
        {
            Param = "start from here"
        };
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _categoryService.GetAllCategories(_userSettings.GetApiUrl(), _sut.Param, 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _categoryService.GetAllCategories(_userSettings.GetApiUrl(), _sut.Param, 73).Returns(Result<List<WikiPageModel>>.Failure("failed to get pages"));

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be("failed to get pages");
    }

    [TearDown]
    public void TearDown()
    {
        _sut.CanMake.Should().BeTrue();
        _sut.Title.Should().Be("All Categories");
        _sut.ParamTitle.Should().Be("Start from");
    }
}
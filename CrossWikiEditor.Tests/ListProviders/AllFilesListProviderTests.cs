namespace CrossWikiEditor.Tests.ListProviders;

public sealed class AllFilesListProviderTests : ListProvidersBaseTest<AllFilesListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _sut = new AllFilesListProvider(_dialogService, _pageService, _userPreferencesService)
        {
            Param = "start from here"
        };
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _pageService.GetAllFiles(_userPrefs.UrlApi(), _sut.Param, 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetAllFiles(_userPrefs.UrlApi(), _sut.Param, 73).Returns(Result<List<WikiPageModel>>.Failure("failed to get pages"));

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
        _sut.Title.Should().Be("All Files");
        _sut.ParamTitle.Should().Be("Start from");
    }
}
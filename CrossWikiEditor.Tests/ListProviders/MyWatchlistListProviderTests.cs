namespace CrossWikiEditor.Tests.ListProviders;

public sealed class MyWatchlistListProviderTests : ListProvidersBaseTest<MyWatchlistListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _sut = new MyWatchlistListProvider(_dialogService, _userService);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _userService.GetWatchlistPages(73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _userService.GetWatchlistPages(73).Returns(Result<List<WikiPageModel>>.Failure("failed to get pages"));

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
        _sut.Title.Should().Be("My watchlist");
        _sut.ParamTitle.Should().Be("");
    }
}